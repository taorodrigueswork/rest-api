using Entities.DTO.Request.Schedule;

namespace Tests.Business;

using global::Business;
using Moq;
using Persistence.Interfaces;


[TestFixture]
public class ScheduleBusinessTests
{
    private Fixture? _fixture;
    private IMapper _mapperMock;
    private ScheduleBusiness? _scheduleBusiness;
    private Mock<IScheduleRepository>? _scheduleRepositoryMock;
    private Mock<IDayRepository>? _dayRepositoryMock;
    private Mock<ILogger<ScheduleBusiness>> _loggerMock;

    [SetUp]
    public void TestInit()
    {
        // AutoMapperMock setup
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapperMock = mapperConfig.CreateMapper();

        _fixture = CustomAutoDataAttribute.CreateOmitOnRecursionFixture();

        // Repository Moq setup
        _scheduleRepositoryMock = new Mock<IScheduleRepository>();
        _dayRepositoryMock = new Mock<IDayRepository>();

        // ILogger mock setup
        _loggerMock = new Mock<ILogger<ScheduleBusiness>>();

        // Subject to test
        _scheduleBusiness = new ScheduleBusiness(_mapperMock, _loggerMock.Object, _scheduleRepositoryMock.Object, _dayRepositoryMock.Object);
    }

    [Test, CustomAutoData]
    public async Task Add_ScheduleDto_ReturnsScheduleEntityAsync(ScheduleDto scheduleDto, ScheduleEntity scheduleEntity)
    {
        // Arrange
        _scheduleRepositoryMock?.Setup(r => r.InsertAsync(It.IsAny<ScheduleEntity>())).ReturnsAsync(scheduleEntity);

        // Act
        var result = await _scheduleBusiness.Add(scheduleDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, scheduleEntity);
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
    }

    [Test, CustomAutoData]
    public async Task Delete_Schedule_by_id_Async(int id, ScheduleEntity scheduleEntity)
    {
        // Arrange
        _scheduleRepositoryMock?.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        await _scheduleBusiness.Delete(id);

        // Assert
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
        _scheduleRepositoryMock?.Verify(p => p.DeleteAsync(It.IsAny<ScheduleEntity>(), null), Times.Once);
        _scheduleRepositoryMock?.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Test, CustomAutoData]
    public void Delete_NotFound_Throws_Exception_Async(int id)
    {
        // Arrange
        ScheduleEntity? scheduleEntity = null;

        _scheduleRepositoryMock?.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(() => _scheduleBusiness.Delete(id));
    }

    [Test, CustomAutoData]
    public async Task GetById_Returns_Schedule_Entity_Async(int id, ScheduleEntity scheduleEntity)
    {
        // Arrange
        _scheduleRepositoryMock?.Setup(p => p.GetByIdWithSubclassesAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        var result = await _scheduleBusiness.GetById(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, scheduleEntity);
        _scheduleRepositoryMock?.Verify(p => p.GetByIdWithSubclassesAsync(It.IsAny<int>()), Times.Once);
    }

    [Test, CustomAutoData]
    public async Task Update_ScheduleDto_Returns_Schedule_Entity_Async(int id, ScheduleDto scheduleDto, ScheduleEntity scheduleEntity)
    {
        // Arrange
        var dayEntityList = _fixture.CreateMany<DayEntity>(3).ToList();
        scheduleEntity.Id = id;

        _scheduleRepositoryMock?.Setup(p => p.GetByIdWithSubclassesAsync(id)).ReturnsAsync(scheduleEntity);
        _dayRepositoryMock?.Setup(p => p.GetDaysAsync(It.IsAny<List<int>>())).ReturnsAsync(dayEntityList);

        // Act
        var result = await _scheduleBusiness.Update(id, scheduleDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
        _scheduleRepositoryMock?.Verify(p => p.UpdateAsync(It.IsAny<ScheduleEntity>(), null), Times.Once);
        _scheduleRepositoryMock?.Verify(p => p.GetByIdWithSubclassesAsync(id), Times.Once);
        _dayRepositoryMock?.Verify(p => p.GetDaysAsync(scheduleDto.Days), Times.Once);
    }

    [Test, CustomAutoData]
    public void Update_NotFound_ReturnsNullAsync(int id, ScheduleDto scheduleDto)
    {
        // Arrange
        ScheduleEntity? scheduleEntity = null;

        _scheduleRepositoryMock?.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(scheduleEntity);

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(() => _scheduleBusiness.Update(id, scheduleDto));
    }
}
