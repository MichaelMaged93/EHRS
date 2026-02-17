using EHRS.Api.Contracts.Doctors;
using EHRS.Api.Helpers;
using EHRS.Api.Localization;
using EHRS.Api.Services;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Dtos.Doctors;
using EHRS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EHRS.Api.Controllers;

[Authorize(Roles = "Doctor")]
[ApiController]
[Route("api/[controller]")]
public sealed class DoctorProfileController : ControllerBase
{
    private readonly IDoctorProfileQueries _queries;
    private readonly IDoctorProfileService _service;
    private readonly IAppLocalizer _loc;

    public DoctorProfileController(
        IDoctorProfileQueries queries,
        IDoctorProfileService service,
        IAppLocalizer loc)
    {
        _queries = queries;
        _service = service;
        _loc = loc;
    }

    [HttpGet]
    public async Task<ActionResult<DoctorProfileDataDto>> Get(CancellationToken ct)
    {
        var doctorId = ClaimsHelper.GetDoctorId(User);

        try
        {
            var result = await _queries.GetDoctorProfileAsync(doctorId, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = _loc["DoctorProfile_NotFound"] });
        }
    }

    [HttpPut]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Put(
        [FromForm] UpdateDoctorProfileForm form,
        CancellationToken ct)
    {
        var doctorId = ClaimsHelper.GetDoctorId(User);

        string? profilePicturePath = null;
        string? certificatePdfPath = null;

        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "uploads",
            "doctors",
            doctorId.ToString());

        Directory.CreateDirectory(uploadsFolder);

        if (form.ProfilePictureFile is not null && form.ProfilePictureFile.Length > 0)
        {
            var ext = Path.GetExtension(form.ProfilePictureFile.FileName);
            var fileName = $"profile_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await form.ProfilePictureFile.CopyToAsync(stream, ct);

            profilePicturePath = $"/uploads/doctors/{doctorId}/{fileName}";
        }

        if (form.CertificatePdfFile is not null && form.CertificatePdfFile.Length > 0)
        {
            var ext = Path.GetExtension(form.CertificatePdfFile.FileName);
            if (!string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { message = _loc["DoctorProfile_CertificateMustBePdf"] });

            var fileName = $"certificate_{Guid.NewGuid():N}.pdf";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            await using var stream = System.IO.File.Create(fullPath);
            await form.CertificatePdfFile.CopyToAsync(stream, ct);

            certificatePdfPath = $"/uploads/doctors/{doctorId}/{fileName}";
        }

        var ok = await _service.UpdateProfileAsync(
            doctorId,
            form.FullName,
            form.MedicalLicense,
            form.Specialization,
            form.Email,
            form.ContactNumber,
            form.Gender,
            form.BirthDate,
            form.AffiliatedHospital,
            form.About,
            profilePicturePath,
            certificatePdfPath,
            ct);

        return ok
            ? NoContent()
            : NotFound(new { message = _loc["DoctorProfile_NotFound"] });
    }
}
