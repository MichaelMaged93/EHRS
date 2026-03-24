using EHRS.Core.Requests.Patients;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatientDtos = EHRS.Core.DTOs.Patients;

namespace EHRS.Core.Abstractions.Queries
{
    public interface IPatientMedicalHistoryQueries
    {
        Task<int> CreateSurgeryAsync(int patientId, int doctorId, CreateSurgeryRequest request);
        Task<bool> DeleteSurgeryAsync(int patientId, int doctorId, int surgeryId);
        Task<PatientDtos.PatientMedicalHistoryDto?> GetMedicalHistoryAsync(int patientId);
        Task<List<PatientDtos.PatientSurgeryDto>> GetSurgeriesAsync(int patientId);
        Task<bool> UpdateMedicalHistoryAsync(int patientId, UpdatePatientMedicalHistoryRequest request);
        Task<bool> UpdateSurgeryAsync(int patientId, int doctorId, int surgeryId, UpdateSurgeryRequest request);
    }
}