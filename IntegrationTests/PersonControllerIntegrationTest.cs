using AutoFixture;
using Entities.DTO.Request.Person;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Persistence.Context;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace IntegrationTests;

[TestClass]
public class PersonControllerIntegrationTest : TestingWebAppFactory
{
    private readonly HttpClient _client;
    private readonly ApiContext _context;
    private readonly Fixture? _fixture;
    private const string ApiV1Person = "/api/v1.0/Person";

    public PersonControllerIntegrationTest()
    {
        WebApplicationFactory<Program> factory = new TestingWebAppFactory();
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApiContext>()!;

        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());// using this property to avoid circular references
    }

    //	Marks a method that should be called after each test method. One such method should be present before each test class.
    [TestCleanup]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
    }

    [TestMethod]
    public async Task GetPersonFromId_Success()
    {
        // Arrange
        _context.Database.EnsureCreated();

        PersonEntity personEntity = _fixture.Create<PersonEntity>();
        personEntity.Id = 1;

        _context.Person?.Add(personEntity);
        _context.SaveChanges();

        // Act
        var response = await _client.GetAsync($"{ApiV1Person}/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = await response.Content.ReadFromJsonAsync<PersonEntity>();
        Assert.AreEqual(personEntity.Id, responseJson?.Id);
        Assert.AreEqual(personEntity.Name, responseJson?.Name);
        Assert.AreEqual(personEntity.Email, responseJson?.Email);
        Assert.AreEqual(personEntity.Phone, responseJson?.Phone);
    }

    [TestMethod]
    public async Task GetPersonFromId_NotFound()
    {
        // Act
        var response = await _client.GetAsync($"{ApiV1Person}/0");

        // Assert
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task AddPerson_ValidInput_Async()
    {
        // Arrange
        PersonDto personDto = _fixture.Create<PersonDto>();
        var jsonContent = JsonConvert.SerializeObject(personDto);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiV1Person, content);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var responseJson = await response.Content.ReadFromJsonAsync<PersonEntity>();

        Assert.IsNotNull(responseJson?.Id);
        Assert.AreEqual(personDto.Name, responseJson?.Name);
        Assert.AreEqual(personDto.Email, responseJson?.Email);
        Assert.AreEqual(personDto.Phone, responseJson?.Phone);
    }

    [TestMethod]
    public async Task AddPerson_InvalidInput_Async()
    {
        // Arrange
        var jsonContent = JsonConvert.SerializeObject(new PersonDto() { Email = "", Name = "", Phone = "" });
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(ApiV1Person, content);

        // Assert
        Assert.AreEqual(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        var responseString = await response.Content.ReadAsStringAsync();

        Assert.IsNotNull(responseString);
        Assert.IsTrue(responseString.Contains("The Name field is required."));
        Assert.IsTrue(responseString.Contains("The Email field is required."));
        Assert.IsTrue(responseString.Contains("The Phone field is required."));
    }
}