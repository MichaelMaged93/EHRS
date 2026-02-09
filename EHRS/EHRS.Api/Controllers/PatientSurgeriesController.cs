using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientSurgeriesController : ControllerBase
{
    private readonly IPatientMedicalHistoryQueries _queries;

    public PatientSurgeriesController(IPatientMedicalHistoryQueries queries)
        => _queries = queries;

    // مؤقت لحد JWT
    private const int PatientId = 10;

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _queries.GetSurgeriesAsync(PatientId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSurgeryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SurgeryType))
            return BadRequest(new { message = "SurgeryType is required." });

        var id = await _queries.CreateSurgeryAsync(PatientId, request);
        return id == 0
            ? BadRequest(new { message = "Invalid data or patient not found." })
            : Ok(new { surgeryId = id });
    }

    [HttpPut("{surgeryId:int}")]
    public async Task<IActionResult> Update([FromRoute] int surgeryId, [FromBody] UpdateSurgeryRequest request)
    {
        var ok = await _queries.UpdateSurgeryAsync(PatientId, surgeryId, request);
        return ok ? NoContent() : NotFound(new { message = "Surgery not found (or invalid update)." });
    }

    [HttpDelete("{surgeryId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int surgeryId)
    {
        var ok = await _queries.DeleteSurgeryAsync(PatientId, surgeryId);
        return ok ? NoContent() : NotFound(new { message = "Surgery not found." });
    }
}
