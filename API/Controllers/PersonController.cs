using Business.Interfaces;
using Entities.DTO.Request.Person;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
public class PersonController : ControllerBase
{
    private readonly ILogger<PersonController> _logger;
    private readonly IBusiness<PersonDTO, PersonEntity> _personBusiness;

    public PersonController(ILogger<PersonController> logger,
        IBusiness<PersonDTO, PersonEntity> personBusiness)
    {
        _logger = logger;
        _personBusiness = personBusiness;
    }

    /// <summary>
    /// Gets a person by their ID.
    /// </summary>
    /// <param name="personId">The ID of the person to get.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the requested entity.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPersonByIdAsync(int id)
    {
        var person = await _personBusiness.GetById(id);
        return person == null ? NotFound() : Ok(person);
    }

    /// <summary>
    /// Adds a new person.
    /// </summary>
    /// <param name="user">The Person object to add.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the new entity created.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PersonEntity))]
    public async Task<IActionResult> AddPersonAsync([FromBody] PersonDTO personDTO)
    {
        return personDTO == null ? BadRequest("Person cannot be null") : Created(string.Empty, await _personBusiness.Add(personDTO));
    }

    /// <summary>
    /// Updates an existing Person.
    /// </summary>
    /// <param name="personId">The ID of the Person to update, passed in a header.</param>
    /// <param name="personDTO">The updated Person object.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the updated entity.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePersonAsync([FromHeader] int personId, [FromBody] PersonDTO personDTO)
    {
        var updatedPerson = await _personBusiness.Update(personId, personDTO);

        return updatedPerson == null ? NotFound() : Ok(updatedPerson);
    }
    /// <summary>
    /// Deletes a person with the specified ID.
    /// </summary>
    /// <param name="personId">The ID of the person to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePersonAsync(int id)
    {
        return await _personBusiness.Delete(id) == null ? NotFound() : NoContent();
    }

}