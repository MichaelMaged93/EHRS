using EHRS.Api.Contracts.Patients;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PatientProfileController : ControllerBase
{
    private readonly IPatientProfileQueries _queries;
    private readonly IWebHostEnvironment _env;

    public PatientProfileController(IPatientProfileQueries queries, IWebHostEnvironment env)
    {
        _queries = queries;
        _env = env;
    }

    // مؤقت لحد JWT
    private const int PatientId = 10;

    [HttpGet]
    public async Task<ActionResult<PatientProfileDto>> Get(CancellationToken ct)
    {
        var dto = await _queries.GetAsync(PatientId, ct);
        if (dto is null) return NotFound("Patient not found.");

        return Ok(dto);
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<PatientProfileDto>> Update(
        [FromForm] UpdatePatientProfileForm form,
        CancellationToken ct)
    {
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
            relativePath = await SavePatientProfilePictureAsync(PatientId, form.ProfilePicture, ct);
        }

        var dto = await _queries.UpdateAsync(PatientId, request, relativePath, ct);
        if (dto is null) return NotFound("Patient not found.");

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
