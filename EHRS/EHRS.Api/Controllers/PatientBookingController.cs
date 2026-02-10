using EHRS.Api.Helpers;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientBookingController : ControllerBase
{
    private readonly IPatientBookingQueries _queries;

    public PatientBookingController(IPatientBookingQueries queries)
    {
        _queries = queries;
    }

    // GET: /api/PatientBooking/areas
    [HttpGet("areas")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetAreas(CancellationToken ct)
    {
        var result = await _queries.GetAreasAsync(ct);
        return Ok(result);
    }

    // GET: /api/PatientBooking/specialties?area=Sohag
    [HttpGet("specialties")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetSpecialties(
        [FromQuery] string area,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(area))
            return BadRequest("area is required.");

        var result = await _queries.GetSpecialtiesAsync(area, ct);
        return Ok(result);
    }

    // GET: /api/PatientBooking/doctors?area=Sohag&specialty=Cardiology
    [HttpGet("doctors")]
    public async Task<ActionResult<IReadOnlyList<PatientBookingDoctorDto>>> GetDoctors(
        [FromQuery] string area,
        [FromQuery] string specialty,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(area))
            return BadRequest("area is required.");

        if (string.IsNullOrWhiteSpace(specialty))
            return BadRequest("specialty is required.");

        var result = await _queries.GetDoctorsAsync(area, specialty, ct);
        return Ok(result);
    }

    // POST: /api/PatientBooking  (Date only)
    [HttpPost]
    public async Task<ActionResult<PatientBookingDto>> Book(
        [FromBody] CreatePatientBookingRequest request,
        CancellationToken ct)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        try
        {
            var result = await _queries.CreateAsync(patientId, request, ct);
            return Created(string.Empty, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
