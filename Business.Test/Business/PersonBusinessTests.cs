namespace Tests.Business;

using Entities.DTO.Request.Person;
using global::Business;
using Persistence.Interfaces;


[TestClass()]
public class PersonBusinessTests
{
    private Fixture? fixture;
    private PersonBusiness? _personBusiness;
    private Mock<IPersonRepository>? _personRepositoryMock;
    private Mock<ILogger<PersonBusiness>> _loggerMock;

    [TestInitialize]
    public void TestInit()
    {
        // Add AutoMapper Profile to avoid duplicated code.
        // The profile is inside the Entities project and is also used in the API project when the project starts.
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());

        fixture = new Fixture();// https://docs.educationsmediagroup.com/unit-testing-csharp/autofixture/quick-glance-at-autofixture
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                         .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());// using this property to avoid circular references

        _personRepositoryMock = new Mock<IPersonRepository>();
        _loggerMock = new Mock<ILogger<PersonBusiness>>();

        _personBusiness = new PersonBusiness(config.CreateMapper(), _loggerMock.Object, _personRepositoryMock.Object);
    }

    [TestMethod]
    public async Task Add_PersonDto_ReturnsPersonEntity()
    {
        // Arrange
        PersonDto personDTO = fixture.Create<PersonDto>();
        PersonEntity personEntity = fixture.Create<PersonEntity>();

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
    public async Task Delete_Person_by_id_ReturnsPersonEntity()
    {
        // Arrange
        var id = fixture.Create<int>();
        PersonEntity personEntity = fixture.Create<PersonEntity>();

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.Delete(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, personEntity);
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Information, _loggerMock.Invocations[0].Arguments[0]);
        _personRepositoryMock.Verify(p => p.DeleteAsync(It.IsAny<PersonEntity>(), null), Times.Once);
        _personRepositoryMock.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public async Task Delete_NotFound_ReturnsNull()
    {
        // Arrange
        var id = fixture.Create<int>();
        PersonEntity? personEntity = null;

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.Delete(id);

        // Assert
        Assert.IsNull(result);
        Assert.AreEqual(1, _loggerMock.Invocations.Count);
        Assert.AreEqual(LogLevel.Warning, _loggerMock.Invocations[0].Arguments[0]);
        _personRepositoryMock.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public async Task GetById_Int_ReturnsPersonEntity()
    {
        // Arrange
        var id = fixture.Create<int>();
        var personEntity = fixture.Create<PersonEntity>();

        _personRepositoryMock.Setup(p => p.FindByIdAsync(id)).ReturnsAsync(personEntity);

        // Act
        var result = await _personBusiness.GetById(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(result, personEntity);
        _personRepositoryMock.Verify(p => p.FindByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [TestMethod]
    public async Task Update_PersonDto_ReturnsPersonEntity()
    {
        // Arrange
        var id = fixture.Create<int>();
        var personDto = fixture.Create<PersonDto>();
        var personEntity = fixture.Create<PersonEntity>();
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
    public async Task Update_NotFound_ReturnsNull()
    {
        // Arrange
        var id = fixture.Create<int>();
        var personDto = fixture.Build<PersonDto>().Create();
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
