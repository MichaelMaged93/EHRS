using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Common;
using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.DoctorAuth;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class DoctorAuthQueries : IDoctorAuthQueries
{
    private readonly EHRSContext _db;

    public DoctorAuthQueries(EHRSContext db) => _db = db;

    public async Task<(bool Success, DoctorAuthError Error)> RegisterAsync(DoctorRegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            return (false, DoctorAuthError.PasswordsDoNotMatch);

        var emailExists = await _db.Doctors.AnyAsync(d => d.Email == request.Email);
        if (emailExists)
            return (false, DoctorAuthError.EmailAlreadyExists);

        var licenseExists = await _db.Doctors.AnyAsync(d => d.MedicalLicense == request.MedicalLicense);
        if (licenseExists)
            return (false, DoctorAuthError.MedicalLicenseAlreadyExists);

        var doctor = new Doctor
        {
            FullName = request.FullName,
            Email = request.Email,
            MedicalLicense = request.MedicalLicense,
            Specialization = request.Specialization,
            ApprovalStatus = 0 // Pending
        };

        _db.Doctors.Add(doctor);
        await _db.SaveChangesAsync();

        var hasher = new PasswordHasher<string>();
        var hash = hasher.HashPassword("Doctor", request.Password);

        var cred = new UserCredential
        {
            Role = "Doctor",
            DoctorId = doctor.DoctorId,
            PasswordHash = hash
        };

        _db.UserCredentials.Add(cred);
        await _db.SaveChangesAsync();

        return (true, DoctorAuthError.None);
    }

    public async Task<(bool Success, DoctorAuthError Error, AuthUserDto? User)> LoginAsync(DoctorLoginRequest request)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Email == request.Email);
        if (doctor is null)
            return (false, DoctorAuthError.InvalidCredentials, null);

        // ApprovalStatus: 0 Pending, 1 Approved, 2 Rejected
        if (doctor.ApprovalStatus == 0)
            return (false, DoctorAuthError.PendingApproval, null);

        if (doctor.ApprovalStatus == 2)
            return (false, DoctorAuthError.Rejected, null);

        var cred = await _db.UserCredentials.FirstOrDefaultAsync(c =>
            c.Role == "Doctor" && c.DoctorId == doctor.DoctorId);

        if (cred is null)
            return (false, DoctorAuthError.InvalidCredentials, null);

        var hasher = new PasswordHasher<string>();
        var verify = hasher.VerifyHashedPassword("Doctor", cred.PasswordHash, request.Password);

        if (verify == PasswordVerificationResult.Failed)
            return (false, DoctorAuthError.InvalidCredentials, null);

        var user = new AuthUserDto
        {
            UserId = doctor.DoctorId,
            Role = "Doctor",
            FullName = doctor.FullName,
            Email = doctor.Email
        };

        return (true, DoctorAuthError.None, user);
    }
}
