using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.PatientAuth;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientAuthQueries
{
    Task<AuthUserDto> RegisterAsync(PatientRegisterRequest request);
    Task<AuthUserDto> LoginAsync(PatientLoginRequest request);
}
