using EHRS.Core.Interfaces;
using EHRS.Core.Requests.ImagingRadiology;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientImagingController : ControllerBase
{
    private readonly IPatientImagingQueries _queries;

    // مؤقت لحد تفعيل JWT
    private const int PatientId = 10;

    public PatientImagingController(IPatientImagingQueries queries) => _queries = queries;

    // GET /api/PatientImaging?page=1&pageSize=10&search=...
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetPatientImagingRequest request)
    {
        var result = await _queries.GetPatientImagingAsync(PatientId, request);
        return Ok(result);
    }

    // GET /api/PatientImaging/5
    [HttpGet("{recordId:int}")]
    public async Task<IActionResult> GetById([FromRoute] int recordId)
    {
        var item = await _queries.GetPatientImagingByRecordIdAsync(PatientId, recordId);
        if (item is null)
            return NotFound(new { message = "Imaging record not found." });

        return Ok(item);
    }
}
