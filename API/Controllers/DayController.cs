using Business.Interfaces;
using Entities.DTO.Request.Day;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DayController : ControllerBase
{
    private readonly IBusiness<DayDTO, DayEntity> _dayBusiness;
    private readonly ILogger<DayController> _logger;


    public DayController(IBusiness<DayDTO, DayEntity> dayBusiness, ILogger<DayController> logger)
    {
        _dayBusiness = dayBusiness;
        _logger = logger;
    }

    /// <summary>
    /// Gets a day by their ID.
    /// </summary>
    /// <param name="dayId">The ID of the day to get.</param>
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
    /// <param name="user">The day object to add.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the new entity created.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(DayEntity))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddDayAsync([FromBody] DayDTO dayDTO)
    {
        return dayDTO == null ? BadRequest("Day cannot be null") : Created(string.Empty, await _dayBusiness.Add(dayDTO));
    }

    /// <summary>
    /// Updates an existing day.
    /// </summary>
    /// <param name="dayId">The ID of the day to update, passed in a header.</param>
    /// <param name="dayDTO">The updated day object.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the updated entity.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DayEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateDayAsync([FromHeader] int dayId, [FromBody] DayDTO dayDTO)
    {
        var updatedDay = await _dayBusiness.Update(dayId, dayDTO);

        return updatedDay == null ? NotFound() : Ok(updatedDay);
    }
    /// <summary>
    /// Deletes a day with the specified ID.
    /// </summary>
    /// <param name="dayId">The ID of the day to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDayAsync(int id)
    {
        return await _dayBusiness.Delete(id) == null ? NotFound() : NoContent();
    }
}
