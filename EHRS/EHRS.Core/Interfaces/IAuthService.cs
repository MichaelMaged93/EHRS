namespace EHRS.Core.Interfaces;

public interface IAuthService
{
    Task<bool> ForgotPasswordAsync(string email);

    Task<bool> VerifyResetCodeAsync(string email, string token);

    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}