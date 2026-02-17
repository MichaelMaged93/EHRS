using EHRS.Core.Common;
using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.DoctorAuth;

namespace EHRS.Core.Abstractions.Queries;

public interface IDoctorAuthQueries
{
    Task<(bool Success, DoctorAuthError Error)> RegisterAsync(DoctorRegisterRequest request);
    Task<(bool Success, DoctorAuthError Error, AuthUserDto? User)> LoginAsync(DoctorLoginRequest request);
}
