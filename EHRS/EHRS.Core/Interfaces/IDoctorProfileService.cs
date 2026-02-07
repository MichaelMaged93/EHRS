namespace EHRS.Core.Interfaces;

public interface IDoctorProfileService
{
    Task<bool> UpdateProfileAsync(
        int doctorId,
        string? fullName,
        string? medicalLicense,
        string? specialization,
        string? email,
        string? contactNumber,
        string? gender,
        DateOnly? birthDate,
        string? affiliatedHospital,
        string? about,
        string? profilePicturePath,
        string? certificatePath,
        CancellationToken ct);
}
