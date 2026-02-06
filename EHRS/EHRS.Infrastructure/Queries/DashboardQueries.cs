using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Dashboard;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries
{
    public sealed class DashboardQueries : IDashboardQueries
    {
        private readonly EHRSContext _db;

        // عدّل الرقم ده لو Status عندكم مختلف
        private const byte CompletedStatusValue = 2;

        public DashboardQueries(EHRSContext db)
        {
            _db = db;
        }

        public async Task<TodayDashboardDto> GetTodayDashboardAsync(
            int doctorId,
            string? status,
            CancellationToken ct)
        {
            var start = DateTime.Today;
            var end = start.AddDays(1);

            var q = _db.Appointments
                .AsNoTracking()
                .Where(a => a.DoctorId == doctorId
                            && a.AppointmentDateTime >= start
                            && a.AppointmentDateTime < end);

            var projected = q.Select(a => new
            {
                a.AppointmentId,
                a.PatientId,
                PatientName = a.Patient.FullName,
                a.IsCancelled,
                a.Status,
                HasMedicalRecord = (a.MedicalRecord != null),
                a.AppointmentDateTime
            });

            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim().ToLowerInvariant();

                projected = projected.Where(x =>
                    (s == "cancelled" && x.IsCancelled) ||
                    (s == "completed" && !x.IsCancelled && (x.Status == CompletedStatusValue || x.HasMedicalRecord)) ||
                    (s == "waiting" && !x.IsCancelled && !(x.Status == CompletedStatusValue || x.HasMedicalRecord))
                );
            }

            var list = await projected.ToListAsync(ct);

            int cancelled = list.Count(x => x.IsCancelled);
            int completed = list.Count(x => !x.IsCancelled && (x.Status == CompletedStatusValue || x.HasMedicalRecord));
            int waiting = list.Count(x => !x.IsCancelled && !(x.Status == CompletedStatusValue || x.HasMedicalRecord));

            return new TodayDashboardDto
            {
                DoctorId = doctorId,
                Date = start,
                CancelledCount = cancelled,
                CompletedCount = completed,
                WaitingCount = waiting,
                Items = list
                    .OrderBy(x => x.AppointmentDateTime)
                    .Select(x => new TodayAppointmentCardDto
                    {
                        AppointmentId = x.AppointmentId,
                        PatientId = x.PatientId,
                        PatientName = x.PatientName ?? "",
                        Status = x.IsCancelled
                            ? "Cancelled"
                            : ((x.Status == CompletedStatusValue || x.HasMedicalRecord) ? "Completed" : "Waiting"),
                        StartAt = x.AppointmentDateTime
                    })
                    .ToList()
            };
        }
    }
}
