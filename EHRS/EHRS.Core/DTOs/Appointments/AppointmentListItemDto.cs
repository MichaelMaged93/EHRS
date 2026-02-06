namespace EHRS.Core.Dtos.Appointments;

public sealed class AppointmentListItemDto
{
    public int AppointmentId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public DateTime AppointmentDateTime { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
