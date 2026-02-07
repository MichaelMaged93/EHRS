namespace EHRS.Core.Dtos.Doctors;

public sealed class DoctorProfileDto
{
    public int DoctorId { get; set; }

    public int TodayAppointments { get; set; }
    public int TodayCancelledAppointments { get; set; }

    public int UpcomingAppointments { get; set; }
    public int PastAppointments { get; set; }

    public int UniquePatients { get; set; }

    public DateTime GeneratedAtUtc { get; set; }
}
