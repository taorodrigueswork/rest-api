using Business;
using Entities.DTO.Request.Day;
using Persistence.Interfaces;

namespace Tests.Business;

[TestFixture]
public class DayBusinessTest
{
    private IMapper? _mapperMock;
    private Mock<IDayRepository>? _dayRepositoryMock;
    private Mock<IPersonRepository>? _personRepositoryMock;
    private Mock<IScheduleRepository>? _scheduleRepositoryMock;
    private Mock<IDayPersonRepository>? _dayPersonRepositoryMock;
    private Mock<ILogger<DayBusiness>>? _loggerMock;
    private DayBusiness? _dayBusiness;

    [SetUp]
    public void TestInit()
    {
        // AutoMapperMock setup
        var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new AutoMapperProfile()));
        _mapperMock = mapperConfig.CreateMapper();

        // Repository Moq setup
        _dayRepositoryMock = new Mock<IDayRepository>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();
        _dayPersonRepositoryMock = new Mock<IDayPersonRepository>();

        // ILogger mock setup
        _loggerMock = new Mock<ILogger<DayBusiness>>();

        // Subject to test
        _dayBusiness = new DayBusiness(_mapperMock, _loggerMock.Object, _dayRepositoryMock.Object, _personRepositoryMock.Object, _scheduleRepositoryMock.Object, _dayPersonRepositoryMock.Object);
    }

    [Test, CustomAutoData]
    public async Task AddNewDayAsync(DayDto dayDto, DayEntity dayEntity)
    {
        // Arrange
        _dayRepositoryMock?.Setup(repo => repo.InsertAsync(It.IsAny<DayEntity>())).ReturnsAsync(dayEntity);

        // Act
        var addedDay = await _dayBusiness?.Add(dayDto)!;

        // Assert
        Assert.IsNotNull(addedDay);
        Assert.AreEqual(addedDay, dayEntity);
        Assert.AreEqual(1, _loggerMock?.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock?.Invocations[0].Arguments[0]);
    }

    [Test, CustomAutoData]
    public async Task Delete_Async(int id, DayEntity dayEntity)
    {
        // Arrange
        _dayRepositoryMock?.Setup(repo => repo.FindByIdAsync(id)).ReturnsAsync(dayEntity);
        _dayRepositoryMock?.Setup(repo => repo.DeleteAsync(It.IsAny<DayEntity>(), null!)).Returns(Task.CompletedTask);

        // Act
        await _dayBusiness?.Delete(id)!;

        // Assert
        _dayRepositoryMock?.Verify(p => p.DeleteAsync(It.IsAny<DayEntity>(), null!), Times.Once);
        _dayRepositoryMock?.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
        Assert.AreEqual(1, _loggerMock?.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock?.Invocations[0].Arguments[0]);
    }

    [Test, CustomAutoData]
    public void Delete_NullDay_Throws_Exception_Async(int id)
    {
        // Arrange
        DayEntity? dayEntity = null;

        _dayRepositoryMock?.Setup(repo => repo.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(dayEntity);

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(() => _dayBusiness?.Delete(id)!);
    }

    [Test, CustomAutoData]
    public async Task GetById_Should_Return_DayEntity_When_Found_Async(int id, DayEntity dayEntity)
    {
        // Arrange
        dayEntity.Id = id;

        _dayRepositoryMock?.Setup(repo => repo.GetDayWithSubclassesAsync(id)).ReturnsAsync(dayEntity);

        // Act
        var result = await _dayBusiness?.GetById(id)!;

        // Assert
        Assert.AreEqual(dayEntity, result);
        _dayRepositoryMock?.Verify(p => p.GetDayWithSubclassesAsync(It.IsAny<int>()), Times.Once);
    }

    [Test, CustomAutoData]
    public async Task UpdatingDay_Async(int dayId, DayDto dayDto, DayEntity dayEntity, List<PersonEntity> personEntities)
    {
        // Arrange
        dayEntity.Id = dayId;

        _dayRepositoryMock?.Setup(repo => repo.GetDayWithSubclassesAsync(dayId)).ReturnsAsync(dayEntity);
        _scheduleRepositoryMock?.Setup(repo => repo.FindByIdAsync(dayDto.ScheduleId)).ReturnsAsync(dayEntity.Schedule);
        _dayPersonRepositoryMock?.Setup(repo => repo.DeleteByDayIdAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        _dayPersonRepositoryMock?.Setup(repo => repo.UpdateAsync(It.IsAny<DayPersonEntity>(), null!)).Returns(Task.CompletedTask);
        _personRepositoryMock?.Setup(repo => repo.GetPeopleAsync(It.IsAny<List<int>>())).ReturnsAsync(personEntities);

        // Act
        var updatedDay = await _dayBusiness?.Update(dayId, dayDto)!;

        // Assert
        Assert.IsNotNull(updatedDay);
        Assert.IsNotNull(updatedDay?.Schedule);
        Assert.AreEqual(updatedDay?.Id, dayId);
        Assert.AreEqual(updatedDay?.Day, dayDto.Day);
        Assert.AreEqual(3, updatedDay?.People.Count);

        _dayRepositoryMock?.Verify(p => p.GetDayWithSubclassesAsync(It.IsAny<int>()), Times.Once);
        _dayPersonRepositoryMock?.Verify(p => p.DeleteByDayIdAsync(It.IsAny<int>()), Times.Once);
        _scheduleRepositoryMock?.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
        _personRepositoryMock?.Verify(p => p.GetPeopleAsync(It.IsAny<List<int>>()), Times.Once);

        Assert.AreEqual(1, _loggerMock?.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock?.Invocations[0].Arguments[0]);
    }

    [Test, CustomAutoData]
    public void Update_Should_LogWarning_When_DayNotFound(int dayId)
    {
        // Arrange
        DayEntity? dayEntity = null;

        _dayRepositoryMock?.Setup(repo => repo.GetDayWithSubclassesAsync(dayId))!.ReturnsAsync(dayEntity);

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(() => _dayBusiness?.Update(dayId, It.IsAny<DayDto>())!);
    }

    [Test, CustomAutoData]
    public async Task Update_Should_Clear_OldPeopleListFromMemory(int dayId, DayEntity dayEntity)
    {
        // Arrange
        dayEntity.Id = dayId;
        dayEntity.Schedule.Id = 1;
        dayEntity.People = new List<PersonEntity> { new PersonEntity() { Id = 1 } };
        var dayDto = new DayDto()
        {
            Day = DateTime.Now,
            People = new List<int> { 2, 3 },
            ScheduleId = 1
        };

        _dayRepositoryMock?.Setup(repo => repo.GetDayWithSubclassesAsync(dayId)).ReturnsAsync(dayEntity);
        _scheduleRepositoryMock?.Setup(repo => repo.FindByIdAsync(dayDto.ScheduleId)).ReturnsAsync(dayEntity.Schedule);
        _dayPersonRepositoryMock?.Setup(repo => repo.DeleteAsync(It.IsAny<DayPersonEntity>(), null!)).Returns(Task.CompletedTask);
        _dayPersonRepositoryMock?.Setup(repo => repo.UpdateAsync(It.IsAny<DayPersonEntity>(), null!)).Returns(Task.CompletedTask);

        var newPeopleList = new List<PersonEntity>() {
            new PersonEntity() { Id = 2 },
            new PersonEntity() { Id = 3 }
        };

        _personRepositoryMock?.Setup(repo => repo.GetPeopleAsync(dayDto.People)).ReturnsAsync(newPeopleList);

        // Act
        var updatedDay = await _dayBusiness?.Update(dayId, dayDto)!;

        // Assert
        Assert.IsNotNull(updatedDay);
        Assert.AreEqual(1, updatedDay?.Schedule.Id);
        Assert.AreEqual(updatedDay?.Id, dayId);
        Assert.AreEqual(updatedDay?.Day, dayDto.Day);
        Assert.AreEqual(2, updatedDay?.People.Count);
        Assert.IsNotNull(updatedDay?.People.Find(x => x.Id == 2));
        Assert.IsNotNull(updatedDay?.People.Find(x => x.Id == 3));
    }
}

