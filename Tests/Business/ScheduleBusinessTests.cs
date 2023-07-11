namespace Tests.Business;

using Entities.DTO.Request.Schedule;
using global::Business;
using Persistence.Interfaces;


[TestClass()]
public class ScheduleBusinessTests
{
    private Fixture? _fixture;
    private IMapper _mapperMock;
    private ScheduleBusiness? _scheduleBusiness;
    private Mock<IScheduleRepository>? _scheduleRepositoryMock;
    private Mock<IDayRepository>? _dayRepositoryMock;
    private Mock<ILogger<ScheduleBusiness>> _loggerMock;

    [TestInitialize]
    public void TestInit()
    {
        // AutoMapperMock setup
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapperMock = mapperConfig.CreateMapper();

        _fixture = new Fixture();// https://docs.educationsmediagroup.com/unit-testing-csharp/autofixture/quick-glance-at-autofixture
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                         .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());// using this property to avoid circular references

        // Repository Moq setup
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();
        _dayRepositoryMock = new Mock<IDayRepository>();

        // ILogger mock setup
        _loggerMock = new Mock<ILogger<ScheduleBusiness>>();

        // Subject to test
        _scheduleBusiness = new ScheduleBusiness(_mapperMock, _loggerMock.Object, _scheduleRepositoryMock.Object, _dayRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Add_ScheduleDto_ReturnsScheduleEntityAsync()
    {
        // Arrange
        ScheduleDto scheduleDTO = _fixture.Create<ScheduleDto>();
        ScheduleEntity scheduleEntity = _fixture.Create<ScheduleEntity>();

        _scheduleRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<ScheduleEntity>())).ReturnsAsync(scheduleEntity);

        // Act
        var result = await _scheduleBusiness.Add(scheduleDTO);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, scheduleEntity);
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
    }

    [TestMethod]
    public async Task Delete_Schedule_by_id_Async()
    {
        // Arrange
        var id = _fixture.Create<int>();
        ScheduleEntity scheduleEntity = _fixture.Create<ScheduleEntity>();

        _scheduleRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        await _scheduleBusiness.Delete(id);

        // Assert
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
        _scheduleRepositoryMock.Verify(p => p.DeleteAsync(It.IsAny<ScheduleEntity>(), null), Times.Once);
        _scheduleRepositoryMock.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task Delete_NotFound_Throws_Exception_Async()
    {
        // Arrange
        var id = _fixture.Create<int>();
        ScheduleEntity? scheduleEntity = null;

        _scheduleRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        await _scheduleBusiness.Delete(id);
    }

    [TestMethod]
    public async Task GetById_Returns_Schedule_Entity_Async()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var scheduleEntity = _fixture.Create<ScheduleEntity>();

        _scheduleRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        var result = await _scheduleBusiness.GetById(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, scheduleEntity);
        _scheduleRepositoryMock.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public async Task Update_ScheduleDto_Returns_Schedule_Entity_Async()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var scheduleDto = _fixture.Create<ScheduleDto>();
        var scheduleEntity = _fixture.Create<ScheduleEntity>();
        scheduleEntity.Id = id;

        _scheduleRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        var result = await _scheduleBusiness.Update(id, scheduleDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
        _scheduleRepositoryMock.Verify(p => p.UpdateAsync(It.IsAny<ScheduleEntity>(), null), Times.Once);
        _dayRepositoryMock.Verify(p => p.GetDaysAsync(scheduleDto.Days), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task Update_NotFound_ReturnsNullAsync()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var ScheduleDto = _fixture.Build<ScheduleDto>().Create();
        ScheduleEntity? scheduleEntity = null;

        _scheduleRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        await _scheduleBusiness.Update(id, ScheduleDto);
    }
}
