using EHRS.Api.Contracts.Doctors;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Dtos.Doctors;
using EHRS.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DoctorProfileController : ControllerBase
{
    private readonly IDoctorProfileQueries _queries;
    private readonly IDoctorProfileService _service;

    public DoctorProfileController(IDoctorProfileQueries queries, IDoctorProfileService service)
    {
        _queries = queries;
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<DoctorProfileDataDto>> Get(CancellationToken ct)
    {
        int doctorId = 2; // مؤقتًا لحد التوكن

        try
        {
            var result = await _queries.GetDoctorProfileAsync(doctorId, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Put([FromForm] UpdateDoctorProfileForm form, CancellationToken ct)
    {
        int doctorId = 2; // مؤقتًا لحد التوكن

        // Validate NOT NULL field (زيادة أمان)
        if (string.IsNullOrWhiteSpace(form.MedicalLicense))
            return BadRequest("MedicalLicense is required.");

        string? profilePicturePath = null;
        string? certificatePdfPath = null;

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            "doctors",
            doctorId.ToString()
        );

        Directory.CreateDirectory(uploadsFolder);

        // Profile picture (optional)
        if (form.ProfilePictureFile is not null && form.ProfilePictureFile.Length > 0)
        {
            var ext = Path.GetExtension(form.ProfilePictureFile.FileName);
            var fileName = $"profile_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await form.ProfilePictureFile.CopyToAsync(stream, ct);

            profilePicturePath = $"/uploads/doctors/{doctorId}/{fileName}";
        }

        // Certificate PDF (optional)
        if (form.CertificatePdfFile is not null && form.CertificatePdfFile.Length > 0)
        {
            var ext = Path.GetExtension(form.CertificatePdfFile.FileName);
            if (!string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("CertificatePdfFile must be a PDF.");

            var fileName = $"certificate_{Guid.NewGuid():N}.pdf";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await form.CertificatePdfFile.CopyToAsync(stream, ct);

            certificatePdfPath = $"/uploads/doctors/{doctorId}/{fileName}";
        }

        var ok = await _service.UpdateProfileAsync(
            doctorId: doctorId,
            fullName: form.FullName,
            medicalLicense: form.MedicalLicense,
            specialization: form.Specialization,
            email: form.Email,
            contactNumber: form.ContactNumber,
            gender: form.Gender,
            birthDate: form.BirthDate,
            affiliatedHospital: form.AffiliatedHospital,
            about: form.About,
            profilePicturePath: profilePicturePath,
            certificatePath: certificatePdfPath,
            ct: ct
        );

        return ok ? NoContent() : NotFound();
    }
}
