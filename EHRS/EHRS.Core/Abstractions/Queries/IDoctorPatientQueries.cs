using DoctorPatientDtos = EHRS.Core.DTOs.DoctorPatients;
using System.Threading.Tasks;

namespace EHRS.Core.Abstractions.Queries
{
    public interface IDoctorPatientQueries
    {
        Task<DoctorPatientDtos.PatientMedicalHistoryDto?> GetMedicalRecordsBySsnAsync(string ssn);
        Task<DoctorPatientDtos.PatientSurgeriesDto?> GetSurgeriesBySsnAsync(string ssn);
    }
}