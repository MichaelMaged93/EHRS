using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Prescriptions;
using EHRS.Core.Requests.Prescriptions;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class PatientPrescriptionsQueries : IPatientPrescriptionsQueries
{
    private readonly EHRSContext _db;
    private const int ActiveDays = 30;

    public PatientPrescriptionsQueries(EHRSContext db)
    {
        _db = db;
    }

    public async Task<PatientPrescriptionsPagedResultDto> GetPatientPrescriptionsAsync(
        int patientId,
        GetPatientPrescriptionsRequest request)
    {
        var tab = (request.Tab ?? string.Empty).Trim().ToLowerInvariant();

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        if (pageSize > 50) pageSize = 50;

        var cutoff = DateTime.UtcNow.AddDays(-ActiveDays);

        var baseQuery =
            _db.MedicalRecords
               .AsNoTracking()
               .Where(m =>
                   m.PatientId == patientId &&
                   m.PrescriptionImagePath != null &&
                   m.PrescriptionImagePath != "");

        if (tab == "active")
        {
            baseQuery = baseQuery.Where(m => m.RecordDateTime >= cutoff);
        }
        else if (tab == "past")
        {
            baseQuery = baseQuery.Where(m => m.RecordDateTime < cutoff);
        }
        else
        {
            // invalid tab -> return empty list (Controller already returns 400)
            return new PatientPrescriptionsPagedResultDto
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = 0,
                Items = new List<PatientPrescriptionCardDto>()
            };
        }

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderByDescending(m => m.RecordDateTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new PatientPrescriptionCardDto
            {
                RecordId = m.RecordId,

                // Doctor info (null-safe)
                DoctorName = m.Doctor != null ? m.Doctor.FullName : "Unknown Doctor",
                DoctorSpecialization = m.Doctor != null ? m.Doctor.Specialization : null,

                PrescriptionDate = m.RecordDateTime,

                //  Medical Record details (NEW)
                ChiefComplaint = m.ChiefComplaint,
                Diagnosis = m.Diagnosis,
                Treatment = m.Treatment,

                PrescriptionPath = m.PrescriptionImagePath!,
                DownloadUrl = $"/api/PatientPrescriptions/{m.RecordId}/download"
            })
            .ToListAsync();

        return new PatientPrescriptionsPagedResultDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<PrescriptionFileRefDto?> GetPrescriptionFileRefAsync(int patientId, int recordId)
    {
        return await _db.MedicalRecords
            .AsNoTracking()
            .Where(m =>
                m.RecordId == recordId &&
                m.PatientId == patientId &&
                m.PrescriptionImagePath != null &&
                m.PrescriptionImagePath != "")
            .Select(m => new PrescriptionFileRefDto
            {
                RecordId = m.RecordId,
                PrescriptionPath = m.PrescriptionImagePath!
            })
            .FirstOrDefaultAsync();
    }
}
