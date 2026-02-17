using EHRS.Api.Helpers;
using EHRS.Api.Localization;
using EHRS.Api.Services;
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
    private readonly IAppLocalizer _loc;

    public PatientDashboardController(
        IPatientDashboardQueries queries,
        IAppLocalizer loc)
    {
        _queries = queries;
        _loc = loc;
    }

    [HttpGet]
    public async Task<ActionResult<PatientDashboardDto>> Get(CancellationToken ct)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var dto = await _queries.GetAsync(patientId, ct);
        if (dto is null)
            return NotFound(new { message = _loc["PatientDashboard_PatientNotFound"] });

        return Ok(dto);
    }
}
