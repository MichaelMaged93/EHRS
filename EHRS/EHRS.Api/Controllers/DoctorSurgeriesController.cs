using EHRS.Api.Helpers;
using EHRS.Api.Localization;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Doctor")]
[ApiController]
[Route("api/[controller]")]
public sealed class DoctorSurgeriesController : ControllerBase
{
    private readonly IPatientMedicalHistoryQueries _queries;
    private readonly IAppLocalizer _loc;

    public DoctorSurgeriesController(IPatientMedicalHistoryQueries queries, IAppLocalizer loc)
    {
        _queries = queries;
        _loc = loc;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSurgeryRequest request, [FromQuery] int patientId)
    {
        if (string.IsNullOrWhiteSpace(request.SurgeryType))
            return BadRequest(new { message = _loc["PatientSurgeries_SurgeryTypeRequired"] });

        if (request.SurgeryDate > DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest(new { message = _loc["PatientSurgeries_CannotCreateFutureDate"] });

        var doctorId = ClaimsHelper.GetDoctorId(User);
        var id = await _queries.CreateSurgeryAsync(patientId, doctorId, request);

        return id == 0
            ? BadRequest(new { message = _loc["PatientSurgeries_InvalidDataOrPatientNotFound"] })
            : Ok(new { surgeryId = id });
    }

    [HttpPut("{surgeryId:int}")]
    public async Task<IActionResult> Update([FromRoute] int surgeryId, [FromQuery] int patientId, [FromBody] UpdateSurgeryRequest request)
    {
        var doctorId = ClaimsHelper.GetDoctorId(User);

        if (request.SurgeryDate.HasValue && request.SurgeryDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
            return BadRequest(new { message = _loc["PatientSurgeries_CannotUpdateFutureDate"] });

        var ok = await _queries.UpdateSurgeryAsync(patientId, doctorId, surgeryId, request);
        return ok
            ? NoContent()
            : NotFound(new { message = _loc["PatientSurgeries_NotFoundOrInvalidUpdate"] });
    }

    [HttpDelete("{surgeryId:int}")]
    public async Task<IActionResult> Delete([FromRoute] int surgeryId, [FromQuery] int patientId)
    {
        var doctorId = ClaimsHelper.GetDoctorId(User);
        var ok = await _queries.DeleteSurgeryAsync(patientId, doctorId, surgeryId);
        return ok
            ? NoContent()
            : NotFound(new { message = _loc["PatientSurgeries_NotFound"] });
    }
}