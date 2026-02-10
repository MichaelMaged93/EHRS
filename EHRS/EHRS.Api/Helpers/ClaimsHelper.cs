using System.Security.Claims;

namespace EHRS.Api.Helpers;

public static class ClaimsHelper
{
    public static int GetPatientId(ClaimsPrincipal user)
        => int.Parse(user.FindFirst("patientId")?.Value
            ?? throw new UnauthorizedAccessException("Missing patientId claim."));

    public static int GetDoctorId(ClaimsPrincipal user)
        => int.Parse(user.FindFirst("doctorId")?.Value
            ?? throw new UnauthorizedAccessException("Missing doctorId claim."));
}
