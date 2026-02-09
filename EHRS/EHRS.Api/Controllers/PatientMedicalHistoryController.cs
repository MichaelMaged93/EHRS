using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientMedicalHistoryController : ControllerBase
{
    private readonly IPatientMedicalHistoryQueries _queries;

    public PatientMedicalHistoryController(IPatientMedicalHistoryQueries queries)
        => _queries = queries;

    // مؤقت لحد JWT
    private const int PatientId = 10;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _queries.GetMedicalHistoryAsync(PatientId);
        return result is null ? NotFound(new { message = "Patient not found." }) : Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePatientMedicalHistoryRequest request)
    {
        if (request.ChronicDiseases is null && request.Allergies is null)
            return BadRequest(new { message = "Provide at least chronicDiseases or allergies." });

        var ok = await _queries.UpdateMedicalHistoryAsync(PatientId, request);
        return ok ? NoContent() : NotFound(new { message = "Patient not found." });
    }
}
