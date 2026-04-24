using EHRS.Core.DTOs.MedicalRecords;
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

            return new MedicalRecordDetailsDto
            {
                RecordId = entity.RecordId,
                PatientId = entity.PatientId,
                DoctorId = entity.DoctorId,
                AppointmentId = entity.AppointmentId,
                RecordDateTime = entity.RecordDateTime,
                ChiefComplaint = entity.ChiefComplaint,
                Diagnosis = entity.Diagnosis,
                ClinicalNotes = entity.ClinicalNotes,
                Treatment = entity.Treatment,
                Radiology = entity.Radiology
            };
        }

        // =========================
        // Upload Prescription (FINAL FIX)
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
        // Upload Radiology (FINAL FIX)
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