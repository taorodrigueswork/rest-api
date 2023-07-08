using API.Controllers;
using Business.Interfaces;
using Entities.DTO.Request.Person;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API.Controller;

[TestClass]
public class PersonControllerTests
{
    private Mock<IBusiness<PersonDto, PersonEntity>> _personBusinessMock;
    private Fixture _fixture;
    private IMapper _mapper;
    private PersonController controller;

    [TestInitialize]
    public void TestInit()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                         .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // AutoMapperMock setup
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
        _mapper = mapperConfig.CreateMapper();

        _personBusinessMock = new Mock<IBusiness<PersonDto, PersonEntity>>();
        controller = new PersonController(_personBusinessMock.Object);
    }

    [TestMethod]
    public async Task GetPersonById_ValidPerson_ReturnsOkObjectResult()
    {
        //Arrange
        var expectedPerson = _fixture.Create<PersonEntity>();
        _personBusinessMock.Setup(x => x.GetById(expectedPerson.Id)).ReturnsAsync(expectedPerson);

        //Act
        var result = await controller.GetPersonByIdAsync(expectedPerson.Id);

        //Assert
        _personBusinessMock.Verify(c => c.GetById(It.IsAny<int>()), Times.Once);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okObjectResult = (OkObjectResult)result;
        var actualPerson = (PersonEntity)okObjectResult.Value;
        Assert.AreEqual(expectedPerson, actualPerson);
    }

    [TestMethod]
    public async Task GetPersonById_InvalidPerson_ReturnsNotFoundResult()
    {
        //Arrange
        int invalidId = -1;
        _personBusinessMock.Setup(x => x.GetById(invalidId)).ReturnsAsync((PersonEntity)null);

        //Act
        var result = await controller.GetPersonByIdAsync(invalidId);

        //Assert
        _personBusinessMock.Verify(c => c.GetById(It.IsAny<int>()), Times.Once);
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task AddPerson_ValidPerson_ReturnsCreatedObjectResult()
    {
        //Arrange
        var personDTO = _fixture.Create<PersonDto>();
        var expectedPerson = _mapper.Map<PersonEntity>(personDTO);
        _personBusinessMock.Setup(x => x.Add(personDTO)).ReturnsAsync(expectedPerson);

        //Act
        var result = await controller.AddPersonAsync(personDTO);

        //Assert
        _personBusinessMock.Verify(c => c.Add(It.IsAny<PersonDto>()), Times.Once);
        Assert.IsInstanceOfType(result, typeof(CreatedResult));
        var createdObjectResult = (CreatedResult)result;
        var actualPerson = (PersonEntity)createdObjectResult.Value;
        Assert.AreEqual(expectedPerson, actualPerson);
    }

    [TestMethod]
    public async Task AddPerson_InvalidPerson_ReturnsBadRequestObjectResult()
    {
        //Act
        var result = await controller.AddPersonAsync(null);

        //Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        var badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.AreEqual("Person cannot be null", badRequestObjectResult.Value);
    }

    [TestMethod]
    [Ignore("This test is gonna be implemented in the ValidationFilterAttribute")]
    public async Task AddPerson_InvalidPerson_MissingFields_ReturnsUnprocessableEntity()
    {
    }

    [TestMethod]
    public async Task UpdatePerson_ValidPerson_ReturnsOkObjectResult()
    {
        //Arrange
        var personDTO = _fixture.Create<PersonDto>();
        var expectedPerson = _mapper.Map<PersonEntity>(personDTO);
        int id = expectedPerson.Id;

        _personBusinessMock.Setup(x => x.Update(id, personDTO)).ReturnsAsync(expectedPerson);

        //Act
        var result = await controller.UpdatePersonAsync(id, personDTO);

        //Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okObjectResult = (OkObjectResult)result;
        var actualPerson = (PersonEntity)okObjectResult.Value;
        Assert.AreEqual(expectedPerson, actualPerson);
    }

    [TestMethod]
    public async Task UpdatePerson_InvalidPerson_ReturnsNotFoundResult()
    {
        //Arrange
        int invalidId = -1;

        //Act
        var result = await controller.UpdatePersonAsync(invalidId, null);

        //Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task DeletePerson_ValidPerson_ReturnsNoContentResult()
    {
        //Arrange
        var personDTO = _fixture.Create<PersonDto>();
        var expectedPerson = _mapper.Map<PersonEntity>(personDTO);
        int id = expectedPerson.Id;
        _personBusinessMock.Setup(x => x.Delete(id)).Returns(Task.CompletedTask);

        //Act
        var result = await controller.DeletePersonAsync(id);

        //Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        _personBusinessMock.Verify(b => b.Delete(id), Times.Once);
    }
}
