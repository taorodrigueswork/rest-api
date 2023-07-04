namespace Business.Test;

using Entities.DTO.Request.Person;
using Entities.Entity;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence.Interfaces;
using System.Threading.Tasks;

[TestClass()]
public class PersonBusinessTests
{
    private PersonBusiness? _personBusiness;
    private Mock<IPersonRepository> _personRepositoryMock;
    private Mock<ILogger<PersonBusiness>> _loggerMock;

    [TestInitialize]
    public void Setup()
    {
        // Add AutoMapper Profile to avoid duplicated code.
        // The profile is inside the Entities project and is also used in the API project when the project starts.
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());

        _personRepositoryMock = new Mock<IPersonRepository>();
        _loggerMock = new Mock<ILogger<PersonBusiness>>();

        _personBusiness = new PersonBusiness(config.CreateMapper(), _loggerMock.Object, _personRepositoryMock.Object);
    }


    [TestMethod()]
    public async Task Add_WithValidPersonDTO_ReturnsPersonEntityAsync()
    {
        // Arrange
        var expectedPerson = new PersonEntity
        {
            Name = "John Doe",
            Phone = "1234567890",
            Email = "johndoe@example.com"
        };
        var personDTO = new PersonDTO
        {
            Name = "John Doe",
            Phone = "1234567890",
            Email = "johndoe@example.com"
        };

        _personRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<PersonEntity>())).ReturnsAsync(expectedPerson);
        // Act
        var actualResult = await _personBusiness.Add(personDTO);

        // Assert
        Assert.IsNotNull(actualResult);
        Assert.AreEqual(expectedPerson.Name, actualResult.Name);
        Assert.AreEqual(expectedPerson.Phone, actualResult.Phone);
        Assert.AreEqual(expectedPerson.Email, actualResult.Email);
    }

    //[TestMethod()]
    //public async Task Delete_ExistingPerson_DeletesPerson()
    //{
    //    var personEntity = new PersonEntity
    //    {
    //        Id = 123,
    //        Name = "John Doe",
    //        Phone = "1234567890",
    //        Email = "johndoe@example.com"
    //    };

    //    _personRepositoryMock.Setup(r => r.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(personEntity);
    //    //_personRepositoryMock.Setup(r => r.DeleteAsync(personEntity)).Returns(Task.CompletedTask);

    //    var result = await _personBusiness.Delete(personEntity.Id);

    //    Assert.AreEqual(personEntity, result);
    //    _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), personEntity), Times.Once);
    //}

    [TestMethod()]
    public async Task GetById_ExistingPerson_ReturnsPerson()
    {
        var personEntity = new PersonEntity
        {
            Id = 123,
            Name = "John Doe",
            Phone = "1234567890",
            Email = "johndoe@example.com"
        };

        _personRepositoryMock.Setup(r => r.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(personEntity);

        var result = await _personBusiness.GetById(personEntity.Id);

        Assert.AreEqual(personEntity, result);
    }

    //[Fact]
    //public async Task Update_ExistingPerson_ReturnsPerson()
    //{
    //    var personDTO = new PersonDTO { /* insert valid properties */ };
    //    var personEntity = new PersonEntity { Id = 123, /* insert valid properties */ };

    //    _personRepositoryMock.Setup(r => r.FindByIdAsync(It.IsAny<int>())).ReturnsAsync(personEntity);
    //    _mapperMock.Setup(m => m.Map<PersonEntity>(It.IsAny<PersonDTO>())).Returns(personEntity);
    //    _personRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PersonEntity>())).Returns(Task.CompletedTask);

    //    var result = await _personBusiness.Update(personEntity.Id, personDTO);

    //    Assert.Equal(personEntity, result);
    //    _loggerMock.Verify(l => l.LogInformation(It.IsAny<string>(), personEntity), Times.Once);
    //}

    //[Fact]
    //public async Task Delete_NonExistingPerson_ReturnsNull()
    //{
    //    var result = await _personBusiness.Delete(123);

    //    Assert.Null(result);
    //    _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Once);
    //    _personRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<PersonEntity>()), Times.Never);
    //}

    //[Fact]
    //public async Task Update_NonExistingPerson_ReturnsNull()
    //{
    //    var personDTO = new PersonDTO { /* insert valid properties */ };

    //    var result = await _personBusiness.Update(123, personDTO);

    //    Assert.Null(result);
    //    _loggerMock.Verify(l => l.LogWarning(It.IsAny<string>()), Times.Once);
    //    _personRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PersonEntity>()), Times.Never);
    //}
}
