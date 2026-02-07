namespace EHRS.Core.DTOs.Doctors;

public sealed class DoctorResponse
{
    public int DoctorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Specialization { get; set; }
    public decimal? Salary { get; set; }
    public string? ProfilePicture { get; set; }

    // ✅ Path للـ PDF
    public string? Certificates { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public bool HasCertificates { get; set; }
}
