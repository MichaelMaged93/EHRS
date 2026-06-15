using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.Requests.Appointments;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class AppointmentQueries : IAppointmentQueries
{
    private readonly EHRSContext _db;

    public AppointmentQueries(EHRSContext db)
    {
        _db = db;
    }

    // =========================
    // UPCOMING
    // =========================
    public async Task<PagedResult<AppointmentListItemDto>> GetDoctorUpcomingAppointmentsAsync(
        int doctorId,
        AppointmentQuery q,
        CancellationToken ct)
    {
        var today = DateTime.Today;

        var baseQuery = _db.Appointments
            .AsNoTracking()
            .Where(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDateTime.Date >= today);

        return await BuildResult(baseQuery, q, ct);
    }

    // =========================
    // PAST
    // =========================
    public async Task<PagedResult<AppointmentListItemDto>> GetDoctorPastAppointmentsAsync(
        int doctorId,
        AppointmentQuery q,
        CancellationToken ct)
    {
        var today = DateTime.Today;

        var baseQuery = _db.Appointments
            .AsNoTracking()
            .Where(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDateTime.Date < today);

        return await BuildResult(baseQuery, q, ct);
    }

    // =========================
    // CORE BUILDER
    // =========================
    private async Task<PagedResult<AppointmentListItemDto>> BuildResult(
        IQueryable<Appointment> baseQuery,
        AppointmentQuery q,
        CancellationToken ct)
    {
        // =========================
        // STATUS FILTER
        // =========================
        if (!string.IsNullOrWhiteSpace(q.Status))
        {
            var st = q.Status.Trim().ToLowerInvariant();

            if (st == "cancelled")
            {
                baseQuery = baseQuery.Where(a => a.IsCancelled);
            }
            else if (st == "waiting")
            {
                baseQuery = baseQuery.Where(a =>
                    !a.IsCancelled &&
                    !_db.MedicalRecords.Any(m => m.AppointmentId == a.AppointmentId));
            }
            else if (st == "completed")
            {
                baseQuery = baseQuery.Where(a =>
                    !a.IsCancelled &&
                    _db.MedicalRecords.Any(m => m.AppointmentId == a.AppointmentId));
            }
            else if (st == "missed")
            {
                baseQuery = baseQuery.Where(a =>
                    !a.IsCancelled &&
                    !_db.MedicalRecords.Any(m => m.AppointmentId == a.AppointmentId) &&
                    a.AppointmentDateTime.Date < DateTime.Today);
            }
        }

        // =========================
        // SEARCH
        // =========================
        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();

            baseQuery = baseQuery.Where(a =>
                a.AppointmentId.ToString().Contains(s) ||
                a.Patient.FullName.Contains(s));
        }

        var totalCount = await baseQuery.CountAsync(ct);

        var page = q.Page <= 0 ? 1 : q.Page;
        var pageSize = (q.PageSize <= 0 || q.PageSize > 100) ? 20 : q.PageSize;

        // =========================
        // QUERY
        // =========================
        var rows = await baseQuery
            .OrderByDescending(a => a.AppointmentDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.AppointmentId,
                a.PatientId,
                a.AppointmentDateTime,
                a.IsCancelled,
                a.ReasonForVisit,

                PatientName = a.Patient.FullName,

                Patient = new
                {
                    a.Patient.PatientId,
                    a.Patient.FullName,
                    a.Patient.Gender,
                    a.Patient.BirthDate,
                    a.Patient.BloodType,
                    a.Patient.HeightCm,
                    a.Patient.WeightKg,
                    a.Patient.Address,
                    a.Patient.ProfilePicture
                },

                HasMedicalRecord = _db.MedicalRecords
                    .Any(m => m.AppointmentId == a.AppointmentId)
            })
            .ToListAsync(ct);

        // =========================
        // MAPPING
        // =========================
        var items = rows.Select(r => new AppointmentListItemDto
        {
            AppointmentId = r.AppointmentId,
            PatientId = r.PatientId,
            PatientName = r.PatientName,

            Gender = r.Patient.Gender,
            BirthDate = r.Patient.BirthDate,
            Age = (r.Patient.BirthDate.HasValue && r.Patient.BirthDate.Value.Year > 1900)
                ? DateTime.UtcNow.Year - r.Patient.BirthDate.Value.Year
                : null,

            BloodType = r.Patient.BloodType,
            HeightCm = r.Patient.HeightCm,
            WeightKg = r.Patient.WeightKg,
            Address = r.Patient.Address,
            ProfilePicture = r.Patient.ProfilePicture,

            AppointmentDateTime = r.AppointmentDateTime,
            Type = r.ReasonForVisit ?? string.Empty,

            Status = r.IsCancelled
                ? "cancelled"
                : r.HasMedicalRecord
                    ? "completed"
                    : r.AppointmentDateTime.Date < DateTime.Today
                        ? "missed"
                        : "waiting"
        }).ToList();

        return new PagedResult<AppointmentListItemDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }
}