using Business.Interfaces;
using Entities.DTO.Request.Day;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiVersion("1.0")]
public class DayController : ControllerBase
{
    private readonly IBusiness<DayDto, DayEntity> _dayBusiness;

    public DayController(IBusiness<DayDto, DayEntity> dayBusiness)
    {
        _dayBusiness = dayBusiness;
    }

    /// <summary>
    /// Gets a day by their ID.
    /// </summary>
    /// <param name="id">The ID of the day to get.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the requested entity.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DayEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDayByIdAsync(int id)
    {
        var day = await _dayBusiness.GetById(id);
        return day == null ? NotFound() : Ok(day);
    }

    /// <summary>
    /// Adds a new day.
    /// </summary>
    /// <param name="dayDto">The day object to add.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the new entity created.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DayEntity))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddDayAsync([FromBody] DayDto dayDto)
    {
        return Created(string.Empty, await _dayBusiness.Add(dayDto));
    }

    /// <summary>
    /// Updates an existing day.
    /// </summary>
    /// <param name="id">The ID of the day to update, passed in a header.</param>
    /// <param name="dayDTO">The updated day object.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the updated entity.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DayEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateDayAsync(int id, [FromBody] DayDto dayDTO)
    {
        var updatedDay = await _dayBusiness.Update(id, dayDTO);

        return updatedDay == null ? NotFound() : Ok(updatedDay);
    }
    /// <summary>
    /// Deletes a day with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the day to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDayAsync(int id)
    {
        await _dayBusiness.Delete(id);
        return NoContent();
    }
}
