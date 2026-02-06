using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.Dtos.Appointments;
using EHRS.Core.Requests.Appointments;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class AppointmentQueries : IAppointmentQueries
{
    private readonly EHRSContext _db;

    public AppointmentQueries(EHRSContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AppointmentListItemDto>> GetDoctorAppointmentsAsync(
        int doctorId,
        AppointmentQuery q,
        CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;

        var baseQuery = _db.Appointments
            .AsNoTracking()
            .Where(a => a.DoctorId == doctorId);

        // scope: upcoming | past
        if (string.Equals(q.Scope, "past", StringComparison.OrdinalIgnoreCase))
            baseQuery = baseQuery.Where(a => a.AppointmentDateTime.Date < today);
        else
            baseQuery = baseQuery.Where(a => a.AppointmentDateTime.Date >= today);

        // status filter: waiting / completed / cancelled
        if (!string.IsNullOrWhiteSpace(q.Status))
        {
            var st = q.Status.Trim().ToLowerInvariant();

            if (st == "cancelled")
            {
                baseQuery = baseQuery.Where(a => a.IsCancelled);
            }
            else if (st == "waiting")
            {
                baseQuery = baseQuery.Where(a => a.Status == 0 && !a.IsCancelled);
            }
            else if (st == "completed")
            {
                baseQuery = baseQuery.Where(a => a.Status == 1 && !a.IsCancelled);
            }
            // أي قيمة تانية: نتجاهلها (أو ممكن ترجع BadRequest في Controller لو حابب)
        }

        // search: AppointmentId OR Patient FullName
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

        // Pull minimal raw fields from DB (include IsCancelled)
        var rows = await baseQuery
            .OrderByDescending(a => a.AppointmentDateTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new
            {
                a.AppointmentId,
                a.AppointmentDateTime,
                a.Status,
                a.IsCancelled,
                a.ReasonForVisit,
                PatientName = a.Patient.FullName
            })
            .ToListAsync(ct);

        // Map to DTO (status + isCancelled -> text)
        var items = rows.Select(r => new AppointmentListItemDto
        {
            AppointmentId = r.AppointmentId,
            PatientName = r.PatientName,
            AppointmentDateTime = r.AppointmentDateTime,
            Type = r.ReasonForVisit ?? string.Empty,  // UI "Type" = ReasonForVisit
            Status = MapStatusToText(r.Status, r.IsCancelled)
        }).ToList();

        return new PagedResult<AppointmentListItemDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    // ===== Status Mapping =====
    private static string MapStatusToText(byte status, bool isCancelled)
    {
        if (isCancelled) return "cancelled";

        return status switch
        {
            0 => "waiting",
            1 => "completed",
            _ => "unknown"
        };
    }
}
