using Entities.DTO.Request.Person;

namespace Tests.Business;

using global::Business;
using Persistence.Interfaces;


[TestFixture]
public class PersonBusinessTests
{
    private IMapper _mapperMock;
    private PersonBusiness? _personBusiness;
    private Mock<IPersonRepository>? _personRepositoryMock;
    private Mock<ILogger<PersonBusiness>>? _loggerMock;

    [SetUp]
    public void TestInit()
    {
        // AutoMapperMock setup
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapperMock = mapperConfig.CreateMapper();

        // Repository Moq setup
        _personRepositoryMock = new Mock<IPersonRepository>();

        // ILogger mock setup
        _loggerMock = new Mock<ILogger<PersonBusiness>>();

        // Subject to test
        _personBusiness = new PersonBusiness(_mapperMock, _loggerMock.Object, _personRepositoryMock.Object);
    }

    [Test, CustomAutoData]
    public async Task Add_PersonDto_ReturnsPersonEntityAsync(PersonDto personDto, PersonEntity personEntity)
    {
        // Assert
        _personRepositoryMock?.Setup(r => r.InsertAsync(It.IsAny<PersonEntity>())).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.Add(personDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, personEntity);
        Assert.AreEqual(1, _loggerMock?.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock?.Invocations[0].Arguments[0]);
    }

    [Test, CustomAutoData]
    public async Task Delete_Person_by_id_ReturnsPersonEntityAsync(int id, PersonEntity personEntity)
    {
        // Assert
        _personRepositoryMock?.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        await _personBusiness.Delete(id);

        // Assert
        Assert.AreEqual(1, _loggerMock?.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock?.Invocations[0].Arguments[0]);
        _personRepositoryMock?.Verify(p => p.DeleteAsync(It.IsAny<PersonEntity>(), null), Times.Once);
        _personRepositoryMock?.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Test, CustomAutoData]
    public void Delete_NotFound_Throws_Exception_Async(int id)
    {
        // Arrange
        PersonEntity? personEntity = null;

        _personRepositoryMock?.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _personBusiness.Delete(id));
    }

    [Test, CustomAutoData]
    public async Task GetById_Int_ReturnsPersonEntityAsync(int id, PersonEntity personEntity)
    {
        // Arrange
        _personRepositoryMock?.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.GetById(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, personEntity);
        _personRepositoryMock?.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Test, CustomAutoData]
    public async Task Update_PersonDto_ReturnsPersonEntityAsync(int id, PersonDto personDto, PersonEntity personEntity)
    {
        // Arrange
        personEntity.Id = id;

        _personRepositoryMock?.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.Update(id, personDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(LogLevel.Information, _loggerMock?.Invocations[0].Arguments[0]);
        _personRepositoryMock?.Verify(p => p.UpdateAsync(It.IsAny<PersonEntity>(), null), Times.Once);
    }

    [Test, CustomAutoData]
    public void Update_NotFound_ReturnsNullAsync(int id, PersonDto personDto)
    {
        // Arrange
        PersonEntity? personEntity = null;

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        Assert.ThrowsAsync<ArgumentNullException>(() => _personBusiness.Update(id, personDto));
    }
}
