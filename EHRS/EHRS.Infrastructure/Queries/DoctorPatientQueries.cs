using EHRS.Core.Abstractions.Queries;
using DoctorPatientDtos = EHRS.Core.DTOs.DoctorPatients;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            // جلب البيانات من قاعدة البيانات بدون استخدام دوال C# داخل LINQ
            var patientData = await _db.Patients
                .Where(p => p.Ssn == ssn)
                .Select(p => new
                {
                    p.PatientId,
                    p.FullName,
                    p.BloodType,
                    p.Diseases,
                    p.Allergies,
                    p.HeightCm,
                    p.WeightKg,
                    p.BirthDate,
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

            if (patientData == null) return null;

            // حساب العمر بعد جلب البيانات على الـ client
            int age = patientData.BirthDate.HasValue
                ? CalculateAge(patientData.BirthDate.Value.ToDateTime(TimeOnly.MinValue))
                : 0;

            return new DoctorPatientDtos.PatientMedicalHistoryDto
            {
                PatientId = patientData.PatientId,
                FullName = patientData.FullName,
                BloodType = patientData.BloodType,
                ChronicDiseases = SplitCsv(patientData.Diseases),
                Allergies = SplitCsv(patientData.Allergies),
                HeightCm = patientData.HeightCm,
                WeightKg = patientData.WeightKg,
                Age = age,
                MedicalRecords = patientData.MedicalRecords
            };
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
                            DoctorId = s.DoctorId,
                            Notes = s.Notes
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        #region Helpers

        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
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

        #endregion
    }
}