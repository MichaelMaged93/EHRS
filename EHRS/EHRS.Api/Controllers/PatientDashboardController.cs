using EHRS.Api.Helpers;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientDashboardController : ControllerBase
{
    private readonly IPatientDashboardQueries _queries;

    public PatientDashboardController(IPatientDashboardQueries queries)
    {
        _queries = queries;
    }

    [HttpGet]
    public async Task<ActionResult<PatientDashboardDto>> Get(CancellationToken ct)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var dto = await _queries.GetAsync(patientId, ct);
        if (dto is null) return NotFound("Patient not found.");

        return Ok(dto);
    }
}
