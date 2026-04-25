using EHRS.Core.DTOs.MedicalRecords;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Interfaces;
using EHRS.Core.Requests.MedicalRecords;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Services
{
    public sealed class MedicalRecordService : IMedicalRecordService
    {
        private readonly EHRSContext _db;

        public MedicalRecordService(EHRSContext db)
        {
            _db = db;
        }

        // =========================
        // CREATE MEDICAL RECORD
        // =========================
        public async Task<MedicalRecordDetailsDto> CreateAsync(
            int doctorId,
            CreateMedicalRecordRequest request,
            CancellationToken ct)
        {
            var appointment = await _db.Appointments
                .FirstOrDefaultAsync(a => a.AppointmentId == request.AppointmentId, ct);

            if (appointment is null)
                throw new InvalidOperationException("Appointment not found.");

            if (appointment.DoctorId != doctorId)
                throw new InvalidOperationException("Appointment does not belong to this doctor.");

            if (appointment.PatientId != request.PatientId)
                throw new InvalidOperationException("PatientId does not match the appointment.");

            if (appointment.IsCancelled)
                throw new InvalidOperationException("Cannot create medical record for cancelled appointment.");

            var exists = await _db.MedicalRecords
                .AnyAsync(r => r.AppointmentId == request.AppointmentId, ct);

            if (exists)
                throw new InvalidOperationException("Medical record already exists.");

            var entity = new MedicalRecord
            {
                PatientId = request.PatientId,
                DoctorId = doctorId,
                AppointmentId = request.AppointmentId,
                RecordDateTime = request.RecordDateTime ?? DateTime.Now,
                ChiefComplaint = request.ChiefComplaint,
                Diagnosis = request.Diagnosis,
                ClinicalNotes = request.ClinicalNotes,
                Treatment = request.Treatment,
                Radiology = request.Radiology
            };

            _db.MedicalRecords.Add(entity);

            appointment.Status = 2; // completed

            await _db.SaveChangesAsync(ct);

            return await _db.MedicalRecords
                .Where(r => r.RecordId == entity.RecordId)
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

        // =========================
        // UPLOAD PRESCRIPTION
        // =========================
        public async Task UploadPrescriptionAsync(
            int recordId,
            Stream fileStream,
            string fileName,
            string webRootPath,
            CancellationToken ct)
        {
            var record = await _db.MedicalRecords
                .FirstOrDefaultAsync(r => r.RecordId == recordId, ct)
                ?? throw new InvalidOperationException("Medical record not found.");

            var basePath = Path.Combine(webRootPath, "uploads", "prescriptions");
            Directory.CreateDirectory(basePath);

            var ext = Path.GetExtension(fileName);
            var safeName = $"record-{recordId}{ext}";
            var filePath = Path.Combine(basePath, safeName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(stream, ct);

            record.PrescriptionImagePath = $"/uploads/prescriptions/{safeName}";

            await _db.SaveChangesAsync(ct);
        }

        // =========================
        // UPLOAD RADIOLOGY
        // =========================
        public async Task UploadRadiologyAsync(
            int recordId,
            Stream fileStream,
            string fileName,
            string webRootPath,
            CancellationToken ct)
        {
            var record = await _db.MedicalRecords
                .FirstOrDefaultAsync(r => r.RecordId == recordId, ct)
                ?? throw new InvalidOperationException("Medical record not found.");

            var basePath = Path.Combine(webRootPath, "uploads", "radiology");
            Directory.CreateDirectory(basePath);

            var ext = Path.GetExtension(fileName);
            var safeName = $"record-{recordId}-rad{ext}";
            var filePath = Path.Combine(basePath, safeName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(stream, ct);

            record.Radiology = $"/uploads/radiology/{safeName}";

            await _db.SaveChangesAsync(ct);
        }
    }
}