using EHRS.Api.Helpers;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.Dtos.Appointments;
using EHRS.Core.Requests.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Doctor")]
[ApiController]
[Route("api/[controller]")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IAppointmentQueries _queries;

    public AppointmentsController(IAppointmentQueries queries)
    {
        _queries = queries;
    }

    [HttpGet("upcoming")]
    public async Task<ActionResult<PagedResult<AppointmentListItemDto>>> GetUpcoming(
        [FromQuery] AppointmentQuery query,
        CancellationToken ct)
    {
        var doctorId = ClaimsHelper.GetDoctorId(User);

        var result = await _queries.GetDoctorUpcomingAppointmentsAsync(doctorId, query, ct);
        return Ok(result);
    }

    [HttpGet("past")]
    public async Task<ActionResult<PagedResult<AppointmentListItemDto>>> GetPast(
        [FromQuery] AppointmentQuery query,
        CancellationToken ct)
    {
        var doctorId = ClaimsHelper.GetDoctorId(User);

        var result = await _queries.GetDoctorPastAppointmentsAsync(doctorId, query, ct);
        return Ok(result);
    }
}