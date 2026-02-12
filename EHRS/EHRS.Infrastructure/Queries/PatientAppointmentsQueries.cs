using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class PatientAppointmentsQueries : IPatientAppointmentsQueries
{
    private readonly EHRSContext _db;

    public PatientAppointmentsQueries(EHRSContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<PatientAppointmentCardDto>> GetUpcomingAsync(
        int patientId,
        PatientAppointmentsQuery query,
        CancellationToken ct = default)
    {
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        if (pageSize > 50) pageSize = 50;

        var now = DateTime.UtcNow;

        var baseQuery = _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId
                        && !a.IsCancelled
                        && a.AppointmentDateTime >= now);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            .OrderBy(a => a.AppointmentDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new PatientAppointmentCardDto
            {
                AppointmentId = a.AppointmentId,
                AppointmentDateTime = a.AppointmentDateTime,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,
                DoctorProfilePicture = a.Doctor.ProfilePicture,
                ReasonForVisit = a.ReasonForVisit,

                // ✅ Unified display status:
                // cancelled if IsCancelled = true
                // completed if Status = 1 and not cancelled
                // waiting otherwise (Status = 0 and not cancelled)
                Status = a.IsCancelled
                    ? "cancelled"
                    : a.Status == 1
                        ? "completed"
                        : "waiting"
            })
            .ToListAsync(ct);

        return new PagedResult<PatientAppointmentCardDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<bool> CancelAsync(int patientId, int appointmentId, CancellationToken ct = default)
    {
        var appt = await _db.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId && a.PatientId == patientId, ct);

        if (appt is null) return false;

        // لو already cancelled اعتبرها نجاح (Idempotent)
        if (appt.IsCancelled) return true;

        // نسمح بالإلغاء للمواعيد القادمة فقط (زي UI)
        if (appt.AppointmentDateTime < DateTime.UtcNow)
            return false;

        appt.IsCancelled = true;
        await _db.SaveChangesAsync(ct);

        return true;
    }
}
