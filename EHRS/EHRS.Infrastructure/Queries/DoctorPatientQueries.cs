using EHRS.Core.Abstractions.Queries;
using DoctorPatientDtos = EHRS.Core.DTOs.DoctorPatients;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EHRS.Infrastructure.Queries
{
    public sealed class DoctorPatientQueries : IDoctorPatientQueries
    {
        private readonly EHRSContext _db;

        public DoctorPatientQueries(EHRSContext db)
        {
            _db = db;
        }

        public async Task<DoctorPatientDtos.PatientMedicalHistoryDto?> GetMedicalRecordsBySsnAsync(string ssn)
        {
            return await _db.Patients
                .Where(p => p.Ssn == ssn)
                .Select(p => new DoctorPatientDtos.PatientMedicalHistoryDto
                {
                    PatientId = p.PatientId,
                    FullName = p.FullName,
                    BloodType = p.BloodType,
                    MedicalRecords = p.MedicalRecords
                        .OrderByDescending(r => r.RecordDateTime)
                        .Select(r => new DoctorPatientDtos.MedicalRecordForDoctorDto
                        {
                            RecordId = r.RecordId,
                            DoctorName = r.Doctor.FullName,
                            Diagnosis = r.Diagnosis,
                            Treatment = r.Treatment,
                            RecordDateTime = r.RecordDateTime
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DoctorPatientDtos.PatientSurgeriesDto?> GetSurgeriesBySsnAsync(string ssn)
        {
            return await _db.Patients
                .Where(p => p.Ssn == ssn)
                .Select(p => new DoctorPatientDtos.PatientSurgeriesDto
                {
                    PatientId = p.PatientId,
                    FullName = p.FullName,
                    Surgeries = p.SurgeryHistories
                        .OrderByDescending(s => s.SurgeryDate)
                        .Select(s => new DoctorPatientDtos.SurgeryForDoctorDto
                        {
                            SurgeryId = s.SurgeryId,
                            SurgeryType = s.SurgeryType,
                            SurgeryDate = s.SurgeryDate.ToDateTime(TimeOnly.MinValue),
                            DoctorId = s.DoctorId, // ارجاع DoctorId مباشرة
                            Notes = s.Notes
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}