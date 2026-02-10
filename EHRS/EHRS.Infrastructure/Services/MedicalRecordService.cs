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
                .AsNoTracking()
                .Where(a => a.AppointmentId == request.AppointmentId)
                .Select(a => new { a.AppointmentId, a.DoctorId, a.PatientId })
                .FirstOrDefaultAsync(ct);

            if (appointment is null)
                throw new InvalidOperationException("Appointment not found.");

            if (appointment.DoctorId != doctorId)
                throw new InvalidOperationException("Appointment does not belong to this doctor.");

            if (appointment.PatientId != request.PatientId)
                throw new InvalidOperationException("PatientId does not match the appointment.");

            bool exists = await _db.MedicalRecords
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
            await _db.SaveChangesAsync(ct);

            return await _db.MedicalRecords
                .AsNoTracking()
                .Where(r => r.RecordId == entity.RecordId)
                .Select(r => new MedicalRecordDetailsDto
                {
                    RecordId = r.RecordId,
                    PatientId = r.PatientId,
                    PatientName = r.Patient.FullName,
                    DoctorId = r.DoctorId,
                    AppointmentId = r.AppointmentId,
                    RecordDateTime = r.RecordDateTime,
                    ChiefComplaint = r.ChiefComplaint,
                    Diagnosis = r.Diagnosis,
                    ClinicalNotes = r.ClinicalNotes,
                    Treatment = r.Treatment,
                    Radiology = r.Radiology,
                    PrescriptionImagePath = r.PrescriptionImagePath
                })
                .FirstAsync(ct);
        }

        public async Task UploadPrescriptionAsync(
            int recordId,
            Stream fileStream,
            string fileName,
            string webRootPath,
            CancellationToken ct)
        {
            var record = await _db.MedicalRecords.FindAsync(new object[] { recordId }, ct)
                ?? throw new InvalidOperationException("Medical record not found.");

            if (string.IsNullOrWhiteSpace(webRootPath))
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var dir = Path.Combine(webRootPath, "uploads", "prescriptions");
            Directory.CreateDirectory(dir);

            var ext = Path.GetExtension(fileName);
            var safeName = $"record-{recordId}{ext}";
            var path = Path.Combine(dir, safeName);

            using var outStream = new FileStream(path, FileMode.Create);
            await fileStream.CopyToAsync(outStream, ct);

            record.PrescriptionImagePath = $"/uploads/prescriptions/{safeName}";
            await _db.SaveChangesAsync(ct);
        }

        public async Task UploadRadiologyAsync(
            int recordId,
            Stream fileStream,
            string fileName,
            string webRootPath,
            CancellationToken ct)
        {
            var record = await _db.MedicalRecords.FindAsync(new object[] { recordId }, ct)
                ?? throw new InvalidOperationException("Medical record not found.");

            if (string.IsNullOrWhiteSpace(webRootPath))
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var dir = Path.Combine(webRootPath, "uploads", "radiology");
            Directory.CreateDirectory(dir);

            var ext = Path.GetExtension(fileName);
            var safeName = $"record-{recordId}-rad{ext}";
            var path = Path.Combine(dir, safeName);

            using var outStream = new FileStream(path, FileMode.Create);
            await fileStream.CopyToAsync(outStream, ct);

            record.Radiology = $"/uploads/radiology/{safeName}";
            await _db.SaveChangesAsync(ct);
        }
    }
}
