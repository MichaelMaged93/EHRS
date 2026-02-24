namespace EHRS.Core.Requests.Patients;

public sealed class CreatePatientBookingRequest
{
    public int DoctorId { get; set; }

    // تاريخ فقط
    public DateOnly AppointmentDate { get; set; }

    //  Enforce الشروط اللي قبلها
    public string Area { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;

    public string? ReasonForVisit { get; set; }
}
