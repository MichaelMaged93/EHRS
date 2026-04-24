using EHRS.Core.Interfaces;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;

namespace EHRS.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly EHRSContext _db;
    private readonly IEmailService _emailService;
    private readonly PasswordHasher<object> _passwordHasher;

    public AuthService(EHRSContext db, IEmailService emailService)
    {
        _db = db;
        _emailService = emailService;
        _passwordHasher = new PasswordHasher<object>();
    }

    // 🔐 Forgot Password (OTP)
    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _db.UserCredentials
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x =>
                (x.Patient != null && x.Patient.Email == email) ||
                (x.Doctor != null && x.Doctor.Email == email));

        if (user == null)
            return true;

        // ✅ Generate 6-digit OTP
        var otp = RandomNumberGenerator.GetInt32(100000, 999999).ToString();

        // ✅ Hash OTP before saving
        user.ResetToken = BCrypt.Net.BCrypt.HashPassword(otp);
        user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        await _db.SaveChangesAsync();

        var subject = "EHRS Password Reset Code";

        var body = $@"
        <h2>Password Reset Request</h2>
        <p>Your OTP code is:</p>
        <h1 style='color:blue'>{otp}</h1>
        <p>This code expires in 10 minutes.</p>
        ";

        await _emailService.SendEmailAsync(email, subject, body);

        return true;
    }

    // 🔐 Verify Code
    public async Task<bool> VerifyResetCodeAsync(string email, string token)
    {
        var user = await _db.UserCredentials
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x =>
                (x.Patient != null && x.Patient.Email == email) ||
                (x.Doctor != null && x.Doctor.Email == email));

        if (user == null)
            return false;

        if (string.IsNullOrEmpty(user.ResetToken))
            return false;

        // ✅ Verify hashed OTP
        if (!BCrypt.Net.BCrypt.Verify(token, user.ResetToken))
            return false;

        if (user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            return false;

        return true;
    }

    // 🔐 Reset Password
    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _db.UserCredentials
            .Include(x => x.Patient)
            .Include(x => x.Doctor)
            .FirstOrDefaultAsync(x =>
                (x.Patient != null && x.Patient.Email == email) ||
                (x.Doctor != null && x.Doctor.Email == email));

        if (user == null)
            return false;

        // ✅ Verify OTP again
        if (!BCrypt.Net.BCrypt.Verify(token, user.ResetToken))
            return false;

        if (user.ResetTokenExpiry == null || user.ResetTokenExpiry < DateTime.UtcNow)
            return false;

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

        // clear
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _db.SaveChangesAsync();

        return true;
    }

    // 🔐 Change Password
    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _db.UserCredentials
            .FirstOrDefaultAsync(x => x.CredentialId == userId);

        if (user == null)
            return false;

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            currentPassword
        );

        if (result == PasswordVerificationResult.Failed)
            return false;

        user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);

        await _db.SaveChangesAsync();

        return true;
    }
}