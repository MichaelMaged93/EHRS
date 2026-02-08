namespace EHRS.Core.DTOs.Patients;

public sealed class PatientBookingDoctorDto
{
    public int DoctorId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Area { get; set; }
}
