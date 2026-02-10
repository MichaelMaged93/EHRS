using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Auth;
using EHRS.Core.Requests.PatientAuth;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class PatientAuthQueries : IPatientAuthQueries
{
    private readonly EHRSContext _db;

    public PatientAuthQueries(EHRSContext db) => _db = db;

    public async Task<AuthUserDto> RegisterAsync(PatientRegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
            throw new InvalidOperationException("Passwords do not match.");

        var emailExists = await _db.Patients.AnyAsync(p => p.Email == request.Email);
        if (emailExists)
            throw new InvalidOperationException("Email already exists.");

        var ssnExists = await _db.Patients.AnyAsync(p => p.Ssn == request.NationalId);
        if (ssnExists)
            throw new InvalidOperationException("National ID already exists.");

        var patient = new Patient
        {
            FullName = request.FullName,
            Email = request.Email,
            BirthDate = request.BirthDate,
            Gender = request.Gender,
            Ssn = request.NationalId
        };

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        var hasher = new PasswordHasher<string>();
        var hash = hasher.HashPassword("Patient", request.Password);

        var cred = new UserCredential
        {
            Role = "Patient",
            PatientId = patient.PatientId,
            PasswordHash = hash
        };

        _db.UserCredentials.Add(cred);
        await _db.SaveChangesAsync();

        return new AuthUserDto
        {
            UserId = patient.PatientId,
            Role = "Patient",
            FullName = patient.FullName,
            Email = patient.Email
        };
    }

    public async Task<AuthUserDto> LoginAsync(PatientLoginRequest request)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Email == request.Email);
        if (patient is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var cred = await _db.UserCredentials.FirstOrDefaultAsync(c =>
            c.Role == "Patient" && c.PatientId == patient.PatientId);

        if (cred is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var hasher = new PasswordHasher<string>();
        var verify = hasher.VerifyHashedPassword("Patient", cred.PasswordHash, request.Password);

        if (verify == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new AuthUserDto
        {
            UserId = patient.PatientId,
            Role = "Patient",
            FullName = patient.FullName,
            Email = patient.Email
        };
    }
}
