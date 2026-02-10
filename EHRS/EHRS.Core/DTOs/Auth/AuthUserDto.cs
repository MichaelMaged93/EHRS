namespace EHRS.Core.DTOs.Auth;

public sealed class AuthUserDto
{
    public int UserId { get; set; }          // PatientId or DoctorId
    public string Role { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
}
