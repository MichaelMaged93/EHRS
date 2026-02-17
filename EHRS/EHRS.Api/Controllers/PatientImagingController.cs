using EHRS.Api.Helpers;
using EHRS.Api.Localization;
using EHRS.Api.Services;
using EHRS.Core.Interfaces;
using EHRS.Core.Requests.ImagingRadiology;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientImagingController : ControllerBase
{
    private readonly IPatientImagingQueries _queries;
    private readonly IAppLocalizer _loc;

    public PatientImagingController(
        IPatientImagingQueries queries,
        IAppLocalizer loc)
    {
        _queries = queries;
        _loc = loc;
    }

    // GET /api/PatientImaging?page=1&pageSize=10&search=...
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetPatientImagingRequest request)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var result = await _queries.GetPatientImagingAsync(patientId, request);
        return Ok(result);
    }

    // GET /api/PatientImaging/5
    [HttpGet("{recordId:int}")]
    public async Task<IActionResult> GetById([FromRoute] int recordId)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var item = await _queries.GetPatientImagingByRecordIdAsync(patientId, recordId);
        if (item is null)
            return NotFound(new { message = _loc["PatientImaging_NotFound"] });

        return Ok(item);
    }
}
