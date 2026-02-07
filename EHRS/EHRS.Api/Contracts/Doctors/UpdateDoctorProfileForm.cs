using Microsoft.AspNetCore.Http;

namespace EHRS.Api.Contracts.Doctors;

public sealed class UpdateDoctorProfileForm
{
    public string? FullName { get; set; }
    public string? MedicalLicense { get; set; }

    public string? Specialization { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }

    public string? AffiliatedHospital { get; set; }
    public string? About { get; set; }

    // Files (API only)
    public IFormFile? ProfilePictureFile { get; set; }
    public IFormFile? CertificatePdfFile { get; set; }
}
