using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.DoctorPatients;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EHRS.Infrastructure.Queries
{
    public sealed class DoctorSurgeryQueries : IDoctorSurgeryQueries
    {
        private readonly EHRSContext _db;

        public DoctorSurgeryQueries(EHRSContext db)
        {
            _db = db;
        }

        public async Task<List<DoctorAllSurgeriesDto>> GetSurgeriesByDoctorAsync(int doctorId)
        {
            return await _db.SurgeryHistories
                .AsNoTracking()
                .Where(s => s.DoctorId == doctorId)
                .OrderByDescending(s => s.SurgeryDate)
                .Select(s => new DoctorAllSurgeriesDto
                {
                    SurgeryId = s.SurgeryId,
                    PatientId = s.PatientId,
                    PatientName = s.Patient.FullName,
                    SurgeryType = s.SurgeryType,
                    SurgeryDate = s.SurgeryDate.ToDateTime(TimeOnly.MinValue),
                    Notes = s.Notes
                })
                .ToListAsync();
        }
    }
}