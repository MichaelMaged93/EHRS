// EHRS.Core/DTOs/Dashboard/TodayDashboardDto.cs
namespace EHRS.Core.DTOs.Dashboard
{
    public class TodayDashboardDto
    {
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }          // تاريخ اليوم (Cairo)
        public int WaitingCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public List<TodayAppointmentCardDto> Items { get; set; } = new();
    }
}
