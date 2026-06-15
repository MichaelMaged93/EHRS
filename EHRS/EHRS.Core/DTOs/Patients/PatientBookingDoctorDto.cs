namespace EHRS.Core.DTOs.Patients;

public sealed class PatientBookingDoctorDto
{
    public int DoctorId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Specialization { get; set; }

    public string? Area { get; set; }

    public string? ProfilePicture { get; set; }

    public string? ContactNumber { get; set; }

    public decimal? Salary { get; set; }

    public int? Age { get; set; }

    public string? About { get; set; }
}