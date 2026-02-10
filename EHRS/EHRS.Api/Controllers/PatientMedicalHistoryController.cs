using EHRS.Api.Helpers;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientMedicalHistoryController : ControllerBase
{
    private readonly IPatientMedicalHistoryQueries _queries;

    public PatientMedicalHistoryController(IPatientMedicalHistoryQueries queries)
        => _queries = queries;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var result = await _queries.GetMedicalHistoryAsync(patientId);
        return result is null
            ? NotFound(new { message = "Patient not found." })
            : Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePatientMedicalHistoryRequest request)
    {
        if (request.ChronicDiseases is null && request.Allergies is null)
            return BadRequest(new { message = "Provide at least chronicDiseases or allergies." });

        var patientId = ClaimsHelper.GetPatientId(User);

        var ok = await _queries.UpdateMedicalHistoryAsync(patientId, request);
        return ok
            ? NoContent()
            : NotFound(new { message = "Patient not found." });
    }
}
