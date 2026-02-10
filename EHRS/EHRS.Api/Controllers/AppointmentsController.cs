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

    [HttpGet]
    public async Task<ActionResult<PagedResult<AppointmentListItemDto>>> Get(
        [FromQuery] AppointmentQuery query,
        CancellationToken ct)
    {
        var doctorId = ClaimsHelper.GetDoctorId(User);

        var result = await _queries.GetDoctorAppointmentsAsync(doctorId, query, ct);
        return Ok(result);
    }
}
