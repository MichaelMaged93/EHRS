using EHRS.Core.Common;
using EHRS.Core.Dtos.Appointments;
using EHRS.Core.Requests.Appointments;

namespace EHRS.Core.Abstractions.Queries;

public interface IAppointmentQueries
{
    Task<PagedResult<AppointmentListItemDto>> GetDoctorUpcomingAppointmentsAsync(
        int doctorId,
        AppointmentQuery query,
        CancellationToken ct);

    Task<PagedResult<AppointmentListItemDto>> GetDoctorPastAppointmentsAsync(
        int doctorId,
        AppointmentQuery query,
        CancellationToken ct);
}