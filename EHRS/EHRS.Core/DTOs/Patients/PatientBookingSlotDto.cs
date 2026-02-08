namespace EHRS.Core.DTOs.Patients;

public sealed class PatientBookingSlotDto
{
    // "HH:mm" عشان يبقى سهل للـ UI
    public string Time { get; set; } = string.Empty;
}
