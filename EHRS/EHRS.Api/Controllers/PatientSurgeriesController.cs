using EHRS.Api.Helpers;
using EHRS.Api.Localization;
using EHRS.Core.Abstractions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientSurgeriesController : ControllerBase
{
    private readonly IPatientMedicalHistoryQueries _queries;
    private readonly IAppLocalizer _loc;

    public PatientSurgeriesController(
        IPatientMedicalHistoryQueries queries,
        IAppLocalizer loc)
    {
        _queries = queries;
        _loc = loc;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var patientId = ClaimsHelper.GetPatientId(User);
        var surgeries = await _queries.GetSurgeriesAsync(patientId);
        return Ok(surgeries);
    }
}