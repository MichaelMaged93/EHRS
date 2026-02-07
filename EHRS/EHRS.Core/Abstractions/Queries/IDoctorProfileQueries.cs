using EHRS.Core.Dtos.Doctors;

namespace EHRS.Core.Abstractions.Queries;

public interface IDoctorProfileQueries
{
    Task<DoctorProfileDataDto> GetDoctorProfileAsync(int doctorId, CancellationToken ct);
}
