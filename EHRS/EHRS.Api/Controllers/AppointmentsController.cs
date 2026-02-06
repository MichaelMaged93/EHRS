using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.Dtos.Appointments;
using EHRS.Core.Requests.Appointments;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

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
        // مؤقتًا نحط DoctorId ثابت للتست
        // بعدين هيتجاب من التوكن
        int doctorId = 2;

        var result = await _queries.GetDoctorAppointmentsAsync(doctorId, query, ct);
        return Ok(result);
    }
}
