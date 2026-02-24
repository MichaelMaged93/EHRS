namespace EHRS.Core.DTOs.Patients;

public sealed class PatientBookingDto
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    //  تاريخ فقط
    public DateOnly AppointmentDate { get; set; }

    public byte Status { get; set; }

    public string? ReasonForVisit { get; set; }

    public bool IsCancelled { get; set; }
}
