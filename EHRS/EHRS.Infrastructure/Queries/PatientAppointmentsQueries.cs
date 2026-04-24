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

        var today = DateTime.Today;

        var baseQuery = _db.Appointments
            .AsNoTracking()
            .Where(a =>
                a.PatientId == patientId
                && !a.IsCancelled
                && a.AppointmentDateTime.Date >= today);

        var totalCount = await baseQuery.CountAsync(ct);

        var items = await baseQuery
            // 🔥 ORDERING FIX
            // Waiting (0) فوق - Completed (1) تحت
            .OrderBy(a => a.Status == 2 ? 1 : 0)
            .ThenBy(a => a.AppointmentDateTime)
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

                Status = a.IsCancelled
                    ? "cancelled"
                    : a.Status == 2
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

    public async Task<bool> CancelAsync(
        int patientId,
        int appointmentId,
        CancellationToken ct = default)
    {
        var appt = await _db.Appointments
            .FirstOrDefaultAsync(a =>
                a.AppointmentId == appointmentId &&
                a.PatientId == patientId,
                ct);

        if (appt is null)
            return false;

        if (appt.IsCancelled)
            return false;

        if (appt.Status == 2)
            return false;

        if (appt.AppointmentDateTime.Date < DateTime.Today)
            return false;

        appt.IsCancelled = true;

        await _db.SaveChangesAsync(ct);

        return true;
    }
}