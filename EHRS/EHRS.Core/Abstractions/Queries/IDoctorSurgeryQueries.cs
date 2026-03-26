using EHRS.Core.DTOs.DoctorPatients;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EHRS.Core.Abstractions.Queries
{
    public interface IDoctorSurgeryQueries
    {
        Task<List<DoctorAllSurgeriesDto>> GetSurgeriesByDoctorAsync(int doctorId, string? search);
    }
}