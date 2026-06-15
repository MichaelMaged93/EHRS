using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.DoctorPatients;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries
{
    public sealed class DoctorSurgeryQueries : IDoctorSurgeryQueries
    {
        private readonly EHRSContext _db;

        public DoctorSurgeryQueries(EHRSContext db)
        {
            _db = db;
        }

        public async Task<List<DoctorAllSurgeriesDto>> GetSurgeriesByDoctorAsync(int doctorId, string? search)
        {
            var query = _db.SurgeryHistories
                .AsNoTracking()
                .Include(s => s.Patient)
                .Where(s => s.DoctorId == doctorId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Patient.FullName.Contains(search));
            }

            return await query
                .OrderByDescending(s => s.SurgeryDate)
                .Select(s => new DoctorAllSurgeriesDto
                {
                    SurgeryId = s.SurgeryId,

                    PatientId = s.PatientId,
                    PatientName = s.Patient.FullName,
                    PatientImageUrl = s.Patient.ProfilePicture,

                    Gender = s.Patient.Gender,
                    BirthDate = s.Patient.BirthDate,

                    // ✅ FIXED: safe client-side calculation
                    Age = s.Patient.BirthDate == null
                        ? null
                        : CalculateAge(s.Patient.BirthDate.Value),

                    BloodType = s.Patient.BloodType,
                    HeightCm = s.Patient.HeightCm,
                    WeightKg = s.Patient.WeightKg,
                    Address = s.Patient.Address,

                    SurgeryType = s.SurgeryType,
                    SurgeryDate = s.SurgeryDate.ToDateTime(TimeOnly.MinValue),
                    Notes = s.Notes
                })
                .ToListAsync();
        }

        // 🧠 Age Helper (safe & clean)
        private static int CalculateAge(DateOnly birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;

            if (birthDate.ToDateTime(TimeOnly.MinValue).Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
}