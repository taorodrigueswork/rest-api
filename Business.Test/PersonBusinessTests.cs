namespace Business.Test;

using Entities.DTO.Request.Person;

[TestClass()]
public class PersonBusinessTests
{
    private PersonBusiness? _personBusiness;

    [TestInitialize]
    public void Setup()
    {
        // Add AutoMapper Profile to avoid duplicated code.
        // The profile is inside the Entities project and is also used in the API project when the project starts.
        var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _personBusiness = new PersonBusiness(config.CreateMapper());
    }


    [TestMethod()]
    public void Add_WithValidPersonDTO_ReturnsPersonEntity()
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

        // Act
        var actualResult = _personBusiness.Add(personDTO);

        // Assert
        Assert.IsNotNull(actualResult);
        Assert.AreEqual(expectedPerson.Name, actualResult.Name);
        Assert.AreEqual(expectedPerson.Phone, actualResult.Phone);
        Assert.AreEqual(expectedPerson.Email, actualResult.Email);
    }

    [TestMethod()]
    public void DeleteTest()
    {
        // Arrange
        var id = 1;

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() => _personBusiness.Delete(id));
    }

    [TestMethod()]
    public void UpdateTest()
    {
        // Arrange
        var personDto = new PersonDTO
        {
            Name = "John Doe",
            Phone = "1234567890",
            Email = "johndoe@example.com"
        };

        // Act & Assert
        Assert.ThrowsException<NotImplementedException>(() => _personBusiness.Update(personDto));
    }
}
