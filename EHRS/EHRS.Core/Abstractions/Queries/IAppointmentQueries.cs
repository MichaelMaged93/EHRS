using EHRS.Core.Common;
using EHRS.Core.Dtos.Appointments;
using EHRS.Core.Requests.Appointments;

namespace EHRS.Core.Abstractions.Queries;

public interface IAppointmentQueries
{
    Task<PagedResult<AppointmentListItemDto>> GetDoctorAppointmentsAsync(
        int doctorId,
        AppointmentQuery query,
        CancellationToken ct);
}
