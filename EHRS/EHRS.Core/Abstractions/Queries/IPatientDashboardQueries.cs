using EHRS.Core.DTOs.Patients;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientDashboardQueries
{
    Task<PatientDashboardDto?> GetAsync(int patientId, CancellationToken ct = default);
}
