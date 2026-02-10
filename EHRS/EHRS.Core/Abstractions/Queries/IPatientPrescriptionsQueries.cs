using EHRS.Core.DTOs.Prescriptions;
using EHRS.Core.Requests.Prescriptions;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientPrescriptionsQueries
{
    Task<PatientPrescriptionsPagedResultDto> GetPatientPrescriptionsAsync(
        int patientId,
        GetPatientPrescriptionsRequest request);

    /// <summary>
    /// Returns the prescription file path if the record belongs to the patient and has a prescription attached.
    /// Returns null if not found / not owned / no prescription.
    /// </summary>
    Task<PrescriptionFileRefDto?> GetPrescriptionFileRefAsync(int patientId, int recordId);
}
