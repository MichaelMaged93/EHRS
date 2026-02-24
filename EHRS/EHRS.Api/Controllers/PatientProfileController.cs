using EHRS.Api.Contracts.Patients;
using EHRS.Api.Helpers;
using EHRS.Api.Localization;
using EHRS.Api.Services;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Patient")]
[ApiController]
[Route("api/[controller]")]
public sealed class PatientProfileController : ControllerBase
{
    private readonly IPatientProfileQueries _queries;
    private readonly IWebHostEnvironment _env;
    private readonly IAppLocalizer _loc;

    public PatientProfileController(
        IPatientProfileQueries queries,
        IWebHostEnvironment env,
        IAppLocalizer loc)
    {
        _queries = queries;
        _env = env;
        _loc = loc;
    }

    [HttpGet]
    public async Task<ActionResult<PatientProfileDto>> Get(CancellationToken ct)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        var dto = await _queries.GetAsync(patientId, ct);
        if (dto is null)
            return NotFound(new { message = _loc["PatientProfile_NotFound"] });

        return Ok(dto);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PatientProfileDto>> Update(
        [FromForm] UpdatePatientProfileForm form,
        CancellationToken ct)
    {
        var patientId = ClaimsHelper.GetPatientId(User);

        // نحول الـ Form لـ Core Request (بنفس pattern عندنا)
        var request = new UpdatePatientProfileRequest
        {
            FullName = form.FullName,
            Gender = form.Gender,
            BirthDate = form.BirthDate,

            Email = form.Email,
            ContactNumber = form.ContactNumber,
            Address = form.Address,

            BloodType = form.BloodType,
            HeightCm = form.HeightCm,
            WeightKg = form.WeightKg,

            Ssn = form.Ssn
        };

        string? relativePath = null;

        if (form.ProfilePicture is not null && form.ProfilePicture.Length > 0)
        {
            //  Validation (اختياري)
            const long maxBytes = 5 * 1024 * 1024; // 5MB
            if (form.ProfilePicture.Length > maxBytes)
                return BadRequest(new { message = _loc["PatientProfile_ImageTooLarge"] });

            var contentType = (form.ProfilePicture.ContentType ?? string.Empty).ToLowerInvariant();
            if (contentType is not ("image/jpeg" or "image/png"))
                return BadRequest(new { message = _loc["PatientProfile_InvalidImage"] });

            relativePath = await SavePatientProfilePictureAsync(patientId, form.ProfilePicture, ct);
        }

        var dto = await _queries.UpdateAsync(patientId, request, relativePath, ct);
        if (dto is null)
            return NotFound(new { message = _loc["PatientProfile_NotFound"] });

        return Ok(dto);
    }

    private async Task<string> SavePatientProfilePictureAsync(int patientId, IFormFile file, CancellationToken ct)
    {
        // wwwroot/uploads/patients/{id}/profile/
        var folderRelative = Path.Combine("uploads", "patients", patientId.ToString(), "profile");

        var webRoot = _env.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
            webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var folderAbsolute = Path.Combine(webRoot, folderRelative);
        Directory.CreateDirectory(folderAbsolute);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext)) ext = ".png";

        var fileName = $"profile_{Guid.NewGuid():N}{ext}";
        var fileAbsolute = Path.Combine(folderAbsolute, fileName);

        await using var stream = new FileStream(fileAbsolute, FileMode.Create);
        await file.CopyToAsync(stream, ct);

        return "/" + Path.Combine(folderRelative, fileName).Replace("\\", "/");
    }
}
