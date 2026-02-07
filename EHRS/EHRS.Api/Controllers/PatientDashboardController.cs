using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientDashboardController : ControllerBase
{
    private readonly IPatientDashboardQueries _queries;

    public PatientDashboardController(IPatientDashboardQueries queries)
    {
        _queries = queries;
    }

    // مؤقت لحد JWT
    private const int PatientId = 10;

    [HttpGet]
    public async Task<ActionResult<PatientDashboardDto>> Get(CancellationToken ct)
    {
        var dto = await _queries.GetAsync(PatientId, ct);
        if (dto is null) return NotFound("Patient not found.");

        return Ok(dto);
    }
}
