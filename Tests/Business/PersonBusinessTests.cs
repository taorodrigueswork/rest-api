namespace Tests.Business;

using Entities.DTO.Request.Person;
using global::Business;
using Persistence.Interfaces;


[TestClass()]
public class PersonBusinessTests
{
    private Fixture? _fixture;
    private IMapper _mapperMock;
    private PersonBusiness? _personBusiness;
    private Mock<IPersonRepository>? _personRepositoryMock;
    private Mock<ILogger<PersonBusiness>> _loggerMock;

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
        _personRepositoryMock = new Mock<IPersonRepository>();

        // ILogger mock setup
        _loggerMock = new Mock<ILogger<PersonBusiness>>();

        // Subject to test
        _personBusiness = new PersonBusiness(_mapperMock, _loggerMock.Object, _personRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Add_PersonDto_ReturnsPersonEntityAsync()
    {
        // Arrange
        PersonDto personDTO = _fixture.Create<PersonDto>();
        PersonEntity personEntity = _fixture.Create<PersonEntity>();

        _personRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<PersonEntity>())).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.Add(personDTO);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, personEntity);
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
    }

    [TestMethod]
    public async Task Delete_Person_by_id_ReturnsPersonEntityAsync()
    {
        // Arrange
        var id = _fixture.Create<int>();
        PersonEntity personEntity = _fixture.Create<PersonEntity>();

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        await _personBusiness.Delete(id);

        // Assert
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
        _personRepositoryMock.Verify(p => p.DeleteAsync(It.IsAny<PersonEntity>(), null), Times.Once);
        _personRepositoryMock.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task Delete_NotFound_Throws_Exception_Async()
    {
        // Arrange
        var id = _fixture.Create<int>();
        PersonEntity? personEntity = null;

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        await _personBusiness.Delete(id);
    }

    [TestMethod]
    public async Task GetById_Int_ReturnsPersonEntityAsync()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var personEntity = _fixture.Create<PersonEntity>();

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.GetById(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, personEntity);
        _personRepositoryMock.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public async Task Update_PersonDto_ReturnsPersonEntityAsync()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var personDto = _fixture.Create<PersonDto>();
        var personEntity = _fixture.Create<PersonEntity>();
        personEntity.Id = id;

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.Update(id, personDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
        _personRepositoryMock.Verify(p => p.UpdateAsync(It.IsAny<PersonEntity>(), null), Times.Once);
    }

    [TestMethod]
    public async Task Update_NotFound_ReturnsNullAsync()
    {
        // Arrange
        var id = _fixture.Create<int>();
        var personDto = _fixture.Build<PersonDto>().Create();
        PersonEntity? personEntity = null;

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.Update(id, personDto);

        // Assert
        _personRepositoryMock.Verify(p => p.UpdateAsync(It.IsAny<PersonEntity>(), null), Times.Never);
        Assert.IsNull(result);
        Assert.AreEqual(LogLevel.Warning, _loggerMock.Invocations[0].Arguments[0]);
    }
}
