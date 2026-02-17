using EHRS.Api.Helpers;
using EHRS.Api.Localization;
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
    private readonly IAppLocalizer _t;

    public PatientBookingController(IPatientBookingQueries queries, IAppLocalizer t)
    {
        _queries = queries;
        _t = t;
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
        [FromQuery] string? area,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(area))
            return BadRequest(new { message = _t["Booking_AreaRequired"] });

        var result = await _queries.GetSpecialtiesAsync(area, ct);
        return Ok(result);
    }

    // GET: /api/PatientBooking/doctors?area=Sohag&specialty=Cardiology
    [HttpGet("doctors")]
    public async Task<ActionResult<IReadOnlyList<PatientBookingDoctorDto>>> GetDoctors(
        [FromQuery] string? area,
        [FromQuery] string? specialty,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(area))
            return BadRequest(new { message = _t["Booking_AreaRequired"] });

        if (string.IsNullOrWhiteSpace(specialty))
            return BadRequest(new { message = _t["Booking_SpecialtyRequired"] });

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
            return BadRequest(new { message = MapBookingError(ex.Message) });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = MapBookingError(ex.Message) });
        }
    }

    private string MapBookingError(string rawMessage)
    {
        var m = (rawMessage ?? string.Empty).Trim();

        if (m.Contains("area", StringComparison.OrdinalIgnoreCase))
            return _t["Booking_InvalidArea"];

        if (m.Contains("special", StringComparison.OrdinalIgnoreCase))
            return _t["Booking_InvalidSpecialty"];

        if (m.Contains("doctor", StringComparison.OrdinalIgnoreCase) &&
            m.Contains("not", StringComparison.OrdinalIgnoreCase))
            return _t["Booking_DoctorNotFound"];

        if (m.Contains("already", StringComparison.OrdinalIgnoreCase) &&
            m.Contains("book", StringComparison.OrdinalIgnoreCase))
            return _t["Booking_AlreadyBooked"];

        if (m.Contains("date", StringComparison.OrdinalIgnoreCase) ||
            m.Contains("past", StringComparison.OrdinalIgnoreCase))
            return _t["Booking_InvalidDate"];

        return _t["Common_UnexpectedError"];
    }
}
