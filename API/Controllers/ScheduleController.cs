using Business.Interfaces;
using Entities.DTO.Request.Schedule;
using Entities.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ApiVersion("1.0")]
public class ScheduleController : ControllerBase
{
    private readonly IBusiness<ScheduleDto, ScheduleEntity> _scheduleBusiness;

    public ScheduleController(IBusiness<ScheduleDto, ScheduleEntity> scheduleBusiness)
    {
        _scheduleBusiness = scheduleBusiness;
    }

    /// <summary>
    /// Gets a schedule by their ID.
    /// </summary>
    /// <param name="id">The ID of the schedule to get.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the requested entity.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ScheduleEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScheduleByIdAsync(int id)
    {
        var schedule = await _scheduleBusiness.GetById(id);
        return schedule == null ? NotFound() : Ok(schedule);
    }

    /// <summary>
    /// Adds a new schedule.
    /// </summary>
    /// <param name="scheduleDto">The schedule object to add.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the new entity created.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ScheduleEntity))]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> AddScheduleAsync([FromBody] ScheduleDto scheduleDto)
    {
        return scheduleDto == null ? BadRequest("Schedule cannot be null") : Created(string.Empty, await _scheduleBusiness.Add(scheduleDto));
    }

    /// <summary>
    /// Updates an existing schedule.
    /// </summary>
    /// <param name="id">The ID of the schedule to update, passed in a header.</param>
    /// <param name="scheduleDto">The updated schedule object.</param>
    /// <returns>An IActionResult representing the status of the operation with the details of the updated entity.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ScheduleEntity))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateScheduleAsync(int id, [FromBody] ScheduleDto scheduleDto)
    {
        var updatedSchedule = await _scheduleBusiness.Update(id, scheduleDto);

        return updatedSchedule == null ? NotFound() : Ok(updatedSchedule);
    }
    /// <summary>
    /// Deletes a schedule with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the schedule to delete.</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteScheduleAsync(int id)
    {
        await _scheduleBusiness.Delete(id);
        return NoContent();
    }

}