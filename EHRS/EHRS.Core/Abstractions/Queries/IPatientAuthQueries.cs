using EHRS.Core.Common;
using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.PatientAuth;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientAuthQueries
{
    Task<(bool Success, PatientAuthError Error, AuthUserDto? User)> RegisterAsync(PatientRegisterRequest request);
    Task<(bool Success, PatientAuthError Error, AuthUserDto? User)> LoginAsync(PatientLoginRequest request);
}
