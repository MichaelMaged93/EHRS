namespace EHRS.Core.DTOs.Dashboard
{
    public class TodayAppointmentCardDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public string Status { get; set; } = "";          // Waiting/Completed/Cancelled
        public DateTime StartAt { get; set; }             // وقت الموعد
    }
}
