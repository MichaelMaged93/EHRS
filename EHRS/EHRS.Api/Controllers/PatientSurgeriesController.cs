using EHRS.Api.Helpers;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientSurgeriesController : ControllerBase
{
    private readonly IPatientMedicalHistoryQueries _queries;

    public PatientSurgeriesController(IPatientMedicalHistoryQueries queries)
        => _queries = queries;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var patientId = ClaimsHelper.GetPatientId(User);
        return Ok(await _queries.GetSurgeriesAsync(patientId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSurgeryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SurgeryType))
            return BadRequest(new { message = "SurgeryType is required." });

        var patientId = ClaimsHelper.GetPatientId(User);

        var id = await _queries.CreateSurgeryAsync(patientId, request);
        return id == 0
            ? BadRequest(new { message = "Invalid data or patient not found." })
            : Ok(new { surgeryId = id });
    }

    [HttpPut("{surgeryId:int}")]
    public async Task<IActionResult> Update([FromRoute] int surgeryId, [FromBody] UpdateSurgeryRequest request)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var ok = await _queries.UpdateSurgeryAsync(patientId, surgeryId, request);
        return ok ? NoContent() : NotFound(new { message = "Surgery not found (or invalid update)." });
    }

    [HttpDelete("{surgeryId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int surgeryId)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var ok = await _queries.DeleteSurgeryAsync(patientId, surgeryId);
        return ok ? NoContent() : NotFound(new { message = "Surgery not found." });
    }
}
