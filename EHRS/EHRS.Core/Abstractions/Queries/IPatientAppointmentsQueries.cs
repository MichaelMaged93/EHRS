using EHRS.Core.Common;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientAppointmentsQueries
{
    Task<PagedResult<PatientAppointmentCardDto>> GetUpcomingAsync(
        int patientId,
        PatientAppointmentsQuery query,
        CancellationToken ct = default);

    Task<bool> CancelAsync(int patientId, int appointmentId, CancellationToken ct = default);
}
