using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientProfileQueries
{
    Task<PatientProfileDto?> GetAsync(int patientId, CancellationToken ct = default);

    Task<PatientProfileDto?> UpdateAsync(
        int patientId,
        UpdatePatientProfileRequest request,
        string? newProfilePictureRelativePath,
        CancellationToken ct = default);
}
