using EHRS.Core.Interfaces;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Services;

public sealed class DoctorProfileService : IDoctorProfileService
{
    private readonly EHRSContext _db;

    public DoctorProfileService(EHRSContext db) => _db = db;

    public async Task<bool> UpdateProfileAsync(
        int doctorId,
        string fullName,
        string medicalLicense,
        string? specialization,
        string? email,
        string? contactNumber,
        string? gender,
        DateOnly? birthDate,
        string? affiliatedHospital,
        string? about,
        string? profilePicturePath,
        string? certificatePath,
        CancellationToken ct)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId, ct);
        if (doctor is null) return false;

        doctor.FullName = fullName;
        doctor.MedicalLicense = medicalLicense; // NOT NULL
        doctor.Specialization = specialization;

        doctor.Email = email;
        doctor.ContactNumber = contactNumber;
        doctor.Gender = gender;
        doctor.BirthDate = birthDate;

        doctor.AffiliatedHospital = affiliatedHospital;
        doctor.About = about;

        if (!string.IsNullOrWhiteSpace(profilePicturePath))
            doctor.ProfilePicture = profilePicturePath;

        if (!string.IsNullOrWhiteSpace(certificatePath))
            doctor.Certificates = certificatePath;

        doctor.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }
}
