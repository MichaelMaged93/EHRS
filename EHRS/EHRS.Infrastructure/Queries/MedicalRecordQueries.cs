using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.DTOs.MedicalRecords;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.MedicalRecords;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries
{
    public sealed class MedicalRecordQueries : IMedicalRecordQueries
    {
        private readonly EHRSContext _db;

        public MedicalRecordQueries(EHRSContext db)
        {
            _db = db;
        }

        // ---------------- LIST ----------------
        public async Task<PagedResult<MedicalRecordListItemDto>> GetDoctorMedicalRecordsAsync(
            int doctorId,
            MedicalRecordQuery query,
            CancellationToken ct)
        {
            int page = query.Page < 1 ? 1 : query.Page;
            int pageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var q = _db.MedicalRecords
                .AsNoTracking()
                .Where(r => r.DoctorId == doctorId);

            var totalCount = await q.CountAsync(ct);

            var items = await q
                .OrderByDescending(r => r.RecordDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new MedicalRecordListItemDto
                {
                    RecordId = r.RecordId,
                    PatientId = r.PatientId,
                    PatientName = r.Patient.FullName,
                    RecordDateTime = r.RecordDateTime,
                    Diagnosis = r.Diagnosis,
                    Treatment = r.Treatment
                })
                .ToListAsync(ct);

            return new PagedResult<MedicalRecordListItemDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = items
            };
        }

        // ---------------- BY ID ----------------
        public async Task<MedicalRecordDetailsDto?> GetByIdAsync(int recordId, CancellationToken ct)
        {
            return await _db.MedicalRecords
                .AsNoTracking()
                .Where(r => r.RecordId == recordId)
                .Select(r => new MedicalRecordDetailsDto
                {
                    RecordId = r.RecordId,
                    DoctorId = r.DoctorId,
                    AppointmentId = r.AppointmentId,
                    RecordDateTime = r.RecordDateTime,

                    ChiefComplaint = r.ChiefComplaint,
                    Diagnosis = r.Diagnosis,
                    ClinicalNotes = r.ClinicalNotes,
                    Treatment = r.Treatment,
                    Radiology = r.Radiology,
                    PrescriptionImagePath = r.PrescriptionImagePath,

                    Patient = r.Patient == null ? null : new PatientMedicalViewDto
                    {
                        PatientId = r.Patient.PatientId,
                        FullName = r.Patient.FullName,
                        Gender = r.Patient.Gender,

                        BirthDate = (r.Patient.BirthDate.HasValue && r.Patient.BirthDate.Value.Year > 1900)
                            ? r.Patient.BirthDate
                            : null,

                        Age = (r.Patient.BirthDate.HasValue && r.Patient.BirthDate.Value.Year > 1900)
                            ? DateTime.UtcNow.Year - r.Patient.BirthDate.Value.Year
                            : null,

                        BloodType = r.Patient.BloodType,
                        HeightCm = r.Patient.HeightCm,
                        WeightKg = r.Patient.WeightKg,

                        Address = r.Patient.Address,
                        ProfilePicture = r.Patient.ProfilePicture
                    }
                })
                .FirstOrDefaultAsync(ct);
        }

        // ---------------- BY APPOINTMENT ----------------
        public async Task<MedicalRecordDetailsDto?> GetByAppointmentAsync(int appointmentId, CancellationToken ct)
        {
            return await _db.MedicalRecords
                .AsNoTracking()
                .Where(r => r.AppointmentId == appointmentId)
                .Select(r => new MedicalRecordDetailsDto
                {
                    RecordId = r.RecordId,
                    DoctorId = r.DoctorId,
                    AppointmentId = r.AppointmentId,
                    RecordDateTime = r.RecordDateTime,

                    ChiefComplaint = r.ChiefComplaint,
                    Diagnosis = r.Diagnosis,
                    ClinicalNotes = r.ClinicalNotes,
                    Treatment = r.Treatment,
                    Radiology = r.Radiology,
                    PrescriptionImagePath = r.PrescriptionImagePath,

                    Patient = r.Patient == null ? null : new PatientMedicalViewDto
                    {
                        PatientId = r.Patient.PatientId,
                        FullName = r.Patient.FullName,
                        Gender = r.Patient.Gender,

                        BirthDate = (r.Patient.BirthDate.HasValue && r.Patient.BirthDate.Value.Year > 1900)
                            ? r.Patient.BirthDate
                            : null,

                        Age = (r.Patient.BirthDate.HasValue && r.Patient.BirthDate.Value.Year > 1900)
                            ? DateTime.UtcNow.Year - r.Patient.BirthDate.Value.Year
                            : null,

                        BloodType = r.Patient.BloodType,
                        HeightCm = r.Patient.HeightCm,
                        WeightKg = r.Patient.WeightKg,

                        Address = r.Patient.Address,
                        ProfilePicture = r.Patient.ProfilePicture
                    }
                })
                .FirstOrDefaultAsync(ct);
        }

        // ---------------- BY PATIENT ----------------
        public async Task<List<MedicalRecordDetailsDto>> GetByPatientAsync(
            int doctorId,
            int patientId,
            CancellationToken ct)
        {
            return await _db.MedicalRecords
                .AsNoTracking()
                .Where(r => r.PatientId == patientId && r.DoctorId == doctorId)
                .OrderByDescending(r => r.RecordDateTime)
                .Select(r => new MedicalRecordDetailsDto
                {
                    RecordId = r.RecordId,
                    DoctorId = r.DoctorId,
                    AppointmentId = r.AppointmentId,
                    RecordDateTime = r.RecordDateTime,

                    ChiefComplaint = r.ChiefComplaint,
                    Diagnosis = r.Diagnosis,
                    ClinicalNotes = r.ClinicalNotes,
                    Treatment = r.Treatment,
                    Radiology = r.Radiology,
                    PrescriptionImagePath = r.PrescriptionImagePath,

                    Patient = r.Patient == null ? null : new PatientMedicalViewDto
                    {
                        PatientId = r.Patient.PatientId,
                        FullName = r.Patient.FullName,
                        Gender = r.Patient.Gender,

                        BirthDate = (r.Patient.BirthDate.HasValue && r.Patient.BirthDate.Value.Year > 1900)
                            ? r.Patient.BirthDate
                            : null,

                        Age = (r.Patient.BirthDate.HasValue && r.Patient.BirthDate.Value.Year > 1900)
                            ? DateTime.UtcNow.Year - r.Patient.BirthDate.Value.Year
                            : null,

                        BloodType = r.Patient.BloodType,
                        HeightCm = r.Patient.HeightCm,
                        WeightKg = r.Patient.WeightKg,

                        Address = r.Patient.Address,
                        ProfilePicture = r.Patient.ProfilePicture
                    }
                })
                .ToListAsync(ct);
        }
    }
}