using EHRS.Api.Helpers;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientAppointmentsController : ControllerBase
{
    private readonly IPatientAppointmentsQueries _queries;

    public PatientAppointmentsController(IPatientAppointmentsQueries queries)
    {
        _queries = queries;
    }

    // Upcoming فقط (اللي لسه هيتراح)
    [HttpGet]
    public async Task<ActionResult<PagedResult<PatientAppointmentCardDto>>> GetUpcoming(
        [FromQuery] PatientAppointmentsQuery query,
        CancellationToken ct)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var result = await _queries.GetUpcomingAsync(patientId, query, ct);
        return Ok(result);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var ok = await _queries.CancelAsync(patientId, id, ct);

        if (!ok)
            return BadRequest("Cannot cancel this appointment (not found, not yours, already past, or invalid).");

        return NoContent();
    }
}
