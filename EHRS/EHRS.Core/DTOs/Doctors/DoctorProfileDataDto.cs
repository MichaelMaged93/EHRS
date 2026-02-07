namespace EHRS.Core.Dtos.Doctors;

public sealed class DoctorProfileDataDto
{
    public int DoctorId { get; set; }

    public string FullName { get; set; } = null!;
    public string MedicalLicense { get; set; } = null!; // NOT NULL
    public string? Specialization { get; set; }

    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }

    public string? ProfilePicture { get; set; }   // path
    public string? Certificates { get; set; }     // pdf path

    public string? AffiliatedHospital { get; set; }
    public string? About { get; set; }
}
