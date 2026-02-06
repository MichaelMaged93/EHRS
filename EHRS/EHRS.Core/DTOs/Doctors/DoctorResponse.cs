namespace EHRS.Core.DTOs.Doctors;

public class DoctorResponse
{
    public int DoctorId { get; set; }
    public string FullName { get; set; } = default!;
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Specialization { get; set; }
    public decimal? Salary { get; set; }
    public string? ProfilePicture { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // نخليها بسيطة دلوقتي: لو محتاجين Certificates لاحقًا نعمل Upload endpoint منفصل
    public bool HasCertificates { get; set; }
}
