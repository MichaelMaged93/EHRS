using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using EHRS.Infrastructure.Persistence;

namespace EHRS.Infrastructure.Queries.Patients;

public sealed class PatientProfileQueries : IPatientProfileQueries
{
    private readonly EHRSContext _db;

    public PatientProfileQueries(EHRSContext db)
    {
        _db = db;
    }

    public async Task<PatientProfileDto?> GetAsync(int patientId, CancellationToken ct = default)
    {
        var p = await _db.Patients.FindAsync(new object?[] { patientId }, ct);
        if (p is null) return null;

        return new PatientProfileDto
        {
            PatientId = p.PatientId,
            FullName = p.FullName,
            Gender = p.Gender,
            BirthDate = p.BirthDate,
            Age = CalculateAge(p.BirthDate),

            Email = p.Email,
            ContactNumber = p.ContactNumber,
            Address = p.Address,

            BloodType = p.BloodType,
            HeightCm = p.HeightCm,
            WeightKg = p.WeightKg,

            Ssn = p.Ssn,
            ProfilePicture = p.ProfilePicture
        };
    }

    public async Task<PatientProfileDto?> UpdateAsync(
        int patientId,
        UpdatePatientProfileRequest request,
        string? newProfilePictureRelativePath,
        CancellationToken ct = default)
    {
        var p = await _db.Patients.FindAsync(new object?[] { patientId }, ct);
        if (p is null) return null;

        // Partial update: لو null ما نغيرش
        if (request.FullName is not null) p.FullName = request.FullName;
        if (request.Gender is not null) p.Gender = request.Gender;
        if (request.BirthDate is not null) p.BirthDate = request.BirthDate;

        if (request.Email is not null) p.Email = request.Email;
        if (request.ContactNumber is not null) p.ContactNumber = request.ContactNumber;
        if (request.Address is not null) p.Address = request.Address;

        if (request.BloodType is not null) p.BloodType = request.BloodType;
        if (request.HeightCm is not null) p.HeightCm = request.HeightCm;
        if (request.WeightKg is not null) p.WeightKg = request.WeightKg;

        if (request.Ssn is not null) p.Ssn = request.Ssn;

        if (!string.IsNullOrWhiteSpace(newProfilePictureRelativePath))
            p.ProfilePicture = newProfilePictureRelativePath;

        await _db.SaveChangesAsync(ct);

        return new PatientProfileDto
        {
            PatientId = p.PatientId,
            FullName = p.FullName,
            Gender = p.Gender,
            BirthDate = p.BirthDate,
            Age = CalculateAge(p.BirthDate),

            Email = p.Email,
            ContactNumber = p.ContactNumber,
            Address = p.Address,

            BloodType = p.BloodType,
            HeightCm = p.HeightCm,
            WeightKg = p.WeightKg,

            Ssn = p.Ssn,
            ProfilePicture = p.ProfilePicture
        };
    }

    private static int? CalculateAge(DateOnly? birthDate)
    {
        if (birthDate is null) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Value.Year;

        if (today < birthDate.Value.AddYears(age))
            age--;

        return age;
    }
}
