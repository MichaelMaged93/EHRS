using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class PatientMedicalHistoryQueries : IPatientMedicalHistoryQueries
{
    private readonly EHRSContext _db;

    public PatientMedicalHistoryQueries(EHRSContext db) => _db = db;

    public async Task<PatientMedicalHistoryDto?> GetMedicalHistoryAsync(int patientId)
    {
        var patient = await _db.Patients
            .AsNoTracking()
            .Where(p => p.PatientId == patientId)
            .Select(p => new { p.Diseases, p.Allergies })
            .FirstOrDefaultAsync();

        if (patient is null) return null;

        var surgeries = await GetSurgeriesAsync(patientId);

        return new PatientMedicalHistoryDto
        {
            ChronicDiseases = SplitCsv(patient.Diseases),
            Allergies = SplitCsv(patient.Allergies),
            Surgeries = surgeries
        };
    }

    public async Task<bool> UpdateMedicalHistoryAsync(int patientId, UpdatePatientMedicalHistoryRequest request)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        if (patient is null) return false;

        // null => سيب القديم كما هو
        if (request.ChronicDiseases is not null)
            patient.Diseases = JoinCsv(NormalizeList(request.ChronicDiseases));

        if (request.Allergies is not null)
            patient.Allergies = JoinCsv(NormalizeList(request.Allergies));

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<PatientSurgeryDto>> GetSurgeriesAsync(int patientId)
    {
        return await _db.SurgeryHistories
            .AsNoTracking()
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.SurgeryDate)
            .Select(s => new PatientSurgeryDto
            {
                SurgeryId = s.SurgeryId,
                SurgeryType = s.SurgeryType,
                SurgeryDate = s.SurgeryDate,
                Notes = s.Notes
            })
            .ToListAsync();
    }

    public async Task<int> CreateSurgeryAsync(int patientId, CreateSurgeryRequest request)
    {
        var patientExists = await _db.Patients.AnyAsync(p => p.PatientId == patientId);
        if (!patientExists) return 0;

        var type = request.SurgeryType?.Trim();
        if (string.IsNullOrWhiteSpace(type) || type.Length < 2 || type.Length > 100) return 0;

        if (request.SurgeryDate > DateOnly.FromDateTime(DateTime.UtcNow)) return 0;

        var notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        if (notes is not null && notes.Length > 500) return 0;

        var entity = new SurgeryHistory
        {
            PatientId = patientId,
            SurgeryType = type,
            SurgeryDate = request.SurgeryDate,
            Notes = notes
        };

        _db.SurgeryHistories.Add(entity);
        await _db.SaveChangesAsync();
        return entity.SurgeryId;
    }

    public async Task<bool> UpdateSurgeryAsync(int patientId, int surgeryId, UpdateSurgeryRequest request)
    {
        var entity = await _db.SurgeryHistories
            .FirstOrDefaultAsync(s => s.SurgeryId == surgeryId && s.PatientId == patientId);

        if (entity is null) return false;

        // لازم يبقى فيه حاجة للتعديل
        if (request.SurgeryType is null && request.SurgeryDate is null && request.Notes is null)
            return false;

        if (request.SurgeryType is not null)
        {
            var type = request.SurgeryType.Trim();
            if (string.IsNullOrWhiteSpace(type) || type.Length < 2 || type.Length > 100) return false;
            entity.SurgeryType = type;
        }

        if (request.SurgeryDate.HasValue)
        {
            if (request.SurgeryDate.Value > DateOnly.FromDateTime(DateTime.UtcNow)) return false;
            entity.SurgeryDate = request.SurgeryDate.Value;
        }

        if (request.Notes is not null)
        {
            var notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
            if (notes is not null && notes.Length > 500) return false;
            entity.Notes = notes;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSurgeryAsync(int patientId, int surgeryId)
    {
        var entity = await _db.SurgeryHistories
            .FirstOrDefaultAsync(s => s.SurgeryId == surgeryId && s.PatientId == patientId);

        if (entity is null) return false;

        _db.SurgeryHistories.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    // ---------------- helpers ----------------
    private static List<string> NormalizeList(IEnumerable<string> items)
    {
        return items
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Where(x => x.Length >= 2 && x.Length <= 60)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(50)
            .ToList();
    }

    private static List<string> SplitCsv(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return new List<string>();

        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string? JoinCsv(List<string> items)
        => items.Count == 0 ? null : string.Join(", ", items);
}
