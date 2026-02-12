namespace EHRS.Core.DTOs.Patients;

public sealed class PatientAppointmentCardDto
{
    public int AppointmentId { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorProfilePicture { get; set; }

    public string? ReasonForVisit { get; set; }

    // Status في الداتابيز عندك byte
    public string Status { get; set; } = string.Empty;
}
