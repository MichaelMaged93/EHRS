using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientMedicalHistoryQueries
{
    Task<PatientMedicalHistoryDto?> GetMedicalHistoryAsync(int patientId);

    Task<bool> UpdateMedicalHistoryAsync(int patientId, UpdatePatientMedicalHistoryRequest request);

    Task<List<PatientSurgeryDto>> GetSurgeriesAsync(int patientId);

    Task<int> CreateSurgeryAsync(int patientId, CreateSurgeryRequest request);

    Task<bool> UpdateSurgeryAsync(int patientId, int surgeryId, UpdateSurgeryRequest request);

    Task<bool> DeleteSurgeryAsync(int patientId, int surgeryId);
}
