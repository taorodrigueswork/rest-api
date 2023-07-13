using Entities.DTO.Request.Day;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Persistence.Context;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace IntegrationTests;

[TestFixture]
public class DayControllerIntegrationTest : TestingWebAppFactory
{
    private readonly HttpClient _client;
    private readonly ApiContext _context;
    private readonly Fixture? _fixture;
    private const string ApiV1Day = "/api/v1.0/Day";

    public DayControllerIntegrationTest()
    {
        WebApplicationFactory<Program> factory = new TestingWebAppFactory();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApiContext>()!;

        _fixture = CustomAutoDataAttribute.CreateOmitOnRecursionFixture();
    }

    //	Marks a method that should be called after each test method. One such method should be present before each test class.
    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
    }

    [Test, CustomAutoData]
    public async Task GetDayFromId_Success(DayEntity dayEntity)
    {
        // Arrange
        _context.Database.EnsureCreated();

        dayEntity.Id = 1;

        _context.Day?.Add(dayEntity);
        _context.SaveChanges();

        // Act
        var response = await _client.GetAsync($"{ApiV1Day}/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadFromJsonAsync<DayEntity>();
        Assert.AreEqual(dayEntity.Id, responseJson?.Id);
        Assert.AreEqual(dayEntity.Day, responseJson?.Day);
        Assert.AreEqual(dayEntity.Schedule.Id, responseJson?.Schedule.Id);
        Assert.Greater(dayEntity.People.Count, 1);
    }

    [Test]
    public async Task GetDayFromId_NotFound()
    {
        // Act
        var response = await _client.GetAsync($"{ApiV1Day}/0");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task AddDay_ValidInput_Async()
    {
        // Arrange
        var personEntity = CreatePersonAndScheduleEntity(out var scheduleEntity);

        DayDto dayDto = _fixture?.Build<DayDto>()
                                ?.With(p => p.People, new List<int>() { personEntity.Id })
                                ?.With(p => p.ScheduleId, scheduleEntity.Id)
                                ?.Create();

        // Arrange
        var jsonContent = JsonConvert.SerializeObject(dayDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiV1Day, content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var responseJson = await response.Content.ReadFromJsonAsync<DayEntity>();

        Assert.IsNotNull(responseJson?.Id);
        Assert.GreaterOrEqual(responseJson?.People.Count, 1);
        Assert.AreEqual(dayDto?.Day, responseJson?.Day);
        Assert.AreEqual(dayDto?.ScheduleId, responseJson?.Schedule.Id);
    }

    [Test]
    public async Task AddDay_InvalidInput_Async()
    {
        // Arrange
        var jsonContent = JsonConvert.SerializeObject(
            new DayDto() { ScheduleId = 0, People = new List<int>(), Day = DateTime.Now });
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiV1Day, content);

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.IsNotNull(responseString);
        Assert.IsTrue(responseString.Contains("The field People must be a string or array type with a minimum length of '1'."));
        Assert.IsTrue(responseString.Contains("The field ScheduleId must be between 1"));
    }

    [Test]
    public async Task DeleteDayAsync_ShouldReturn_Status204NoContent_WhenDayExists()
    {
        //Arrange
        var personEntity = CreatePersonAndScheduleEntity(out var scheduleEntity);

        _context.Database.EnsureCreated();

        DayEntity dayEntity = new DayEntity()
        {
            People = new List<PersonEntity>() { personEntity },
            Schedule = scheduleEntity,
            Day = DateTime.Now
        };

        _context.Day?.Add(dayEntity);
        _context.SaveChanges();

        //Act
        var result = await _client.DeleteAsync($"{ApiV1Day}/{dayEntity.Id}");

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
    }

    [Test]
    public async Task DeleteDayAsync_ShouldReturn_Status404NotFound_WhenDayDoesNotExist()
    {
        //Act
        var result = await _client.DeleteAsync($"{ApiV1Day}/1");

        //Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Test]
    public async Task UpdateDayAsync_ReturnsOkObjectResult_WhenDayIsUpdated()
    {
        // Arrange
        var personEntity = CreatePersonAndScheduleEntity(out var scheduleEntity);
        var personEntity2 = CreatePersonAndScheduleEntity(out var scheduleEntity2);

        _context.Database.EnsureCreated();

        DayEntity dayEntity = new DayEntity()
        {
            People = new List<PersonEntity>() { personEntity },
            Schedule = scheduleEntity,
            Day = DateTime.Now
        };

        _context.Day?.Add(dayEntity);
        _context.SaveChanges();

        DayDto dayDto = new()
        {
            People = new List<int>() { personEntity2.Id },
            Day = DateTime.Now,
            ScheduleId = scheduleEntity2.Id
        };

        var jsonContent = JsonConvert.SerializeObject(dayDto);

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"{ApiV1Day}/{dayEntity.Id}", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadFromJsonAsync<DayEntity>();

        Assert.IsNotNull(responseJson);
        Assert.AreEqual(dayDto?.People?.FirstOrDefault(), responseJson?.People?.FirstOrDefault()?.Id);
        Assert.AreNotSame(personEntity?.Id, responseJson?.People?.FirstOrDefault()?.Id);
        Assert.AreEqual(dayDto?.ScheduleId, responseJson?.Schedule?.Id);
        Assert.AreNotSame(scheduleEntity?.Id, responseJson?.Schedule?.Id);
    }

    [Test, CustomAutoData]
    public async Task UpdateDayAsync_ReturnsNotFoundResult_WhenDayDoesNotExist(DayDto dayDto)
    {
        // Arrange
        var jsonContent = JsonConvert.SerializeObject(dayDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"{ApiV1Day}/-1", content);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Test]
    public async Task UpdateDayAsync_ReturnsUnprocessableEntityObjectResult_WhenModelIsInvalid()
    {
        // Arrange
        var jsonContent = JsonConvert.SerializeObject(
            new DayDto() { ScheduleId = 0, People = new List<int>(), Day = DateTime.Now });
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"{ApiV1Day}/1", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.IsNotNull(responseString);
        Assert.IsTrue(responseString.Contains("The field People must be a string or array type with a minimum length of '1'."));
        Assert.IsTrue(responseString.Contains("The field ScheduleId must be between 1"));
    }

    private PersonEntity CreatePersonAndScheduleEntity(out ScheduleEntity scheduleEntity)
    {
        _context.Database.EnsureCreated();

        PersonEntity personEntity = _fixture.Create<PersonEntity>();
        scheduleEntity = _fixture.Create<ScheduleEntity>();

        _context.Person?.Add(personEntity);
        _context.Schedule?.Add(scheduleEntity);

        _context.SaveChanges();
        return personEntity;
    }
}