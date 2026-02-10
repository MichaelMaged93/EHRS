using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.DoctorAuth;

namespace EHRS.Core.Abstractions.Queries;

public interface IDoctorAuthQueries
{
    Task<(bool Success, string Message)> RegisterAsync(DoctorRegisterRequest request);
    Task<AuthUserDto> LoginAsync(DoctorLoginRequest request);
}
