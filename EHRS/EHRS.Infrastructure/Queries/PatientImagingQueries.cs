using EHRS.Core.Common;
using EHRS.Core.DTOs.ImagingRadiology;
using EHRS.Core.Interfaces;
using EHRS.Core.Requests.ImagingRadiology;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class PatientImagingQueries : IPatientImagingQueries
{
    private readonly EHRSContext _db;

    public PatientImagingQueries(EHRSContext db) => _db = db;

    public async Task<PagedResult<PatientImagingListItemDto>> GetPatientImagingAsync(
        int patientId,
        GetPatientImagingRequest request)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

        var query = _db.MedicalRecords
            .AsNoTracking()
            .Where(r => r.PatientId == patientId)
            .Where(r => r.Radiology != null && r.Radiology != "");

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();

            query = query.Where(r =>
                (r.ClinicalNotes != null && r.ClinicalNotes.Contains(s)) ||
                (r.Diagnosis != null && r.Diagnosis.Contains(s)) ||
                (r.ChiefComplaint != null && r.ChiefComplaint.Contains(s)));
        }

        query = query.OrderByDescending(r => r.RecordDateTime);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new PatientImagingListItemDto
            {
                RecordId = r.RecordId,
                RecordDateTime = r.RecordDateTime,
                ClinicalNotes = r.ClinicalNotes,
                RadiologyPath = r.Radiology
            })
            .ToListAsync();

        return new PagedResult<PatientImagingListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PatientImagingListItemDto?> GetPatientImagingByRecordIdAsync(
        int patientId,
        int recordId)
    {
        return await _db.MedicalRecords
            .AsNoTracking()
            .Where(r => r.PatientId == patientId)
            .Where(r => r.RecordId == recordId)
            .Where(r => r.Radiology != null && r.Radiology != "")
            .Select(r => new PatientImagingListItemDto
            {
                RecordId = r.RecordId,
                RecordDateTime = r.RecordDateTime,
                ClinicalNotes = r.ClinicalNotes,
                RadiologyPath = r.Radiology
            })
            .FirstOrDefaultAsync();
    }
}
