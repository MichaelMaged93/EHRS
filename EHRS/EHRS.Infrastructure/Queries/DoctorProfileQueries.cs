using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Dtos.Doctors;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class DoctorProfileQueries : IDoctorProfileQueries
{
    private readonly EHRSContext _db;

    public DoctorProfileQueries(EHRSContext db)
    {
        _db = db;
    }

    public async Task<DoctorProfileDataDto> GetDoctorProfileAsync(int doctorId, CancellationToken ct)
    {
        var dto = await _db.Doctors
            .AsNoTracking()
            .Where(d => d.DoctorId == doctorId)
            .Select(d => new DoctorProfileDataDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                MedicalLicense = d.MedicalLicense,
                Specialization = d.Specialization,
                Email = d.Email,
                ContactNumber = d.ContactNumber,
                Gender = d.Gender,
                BirthDate = d.BirthDate,
                ProfilePicture = d.ProfilePicture,
                Certificates = d.Certificates,
                AffiliatedHospital = d.AffiliatedHospital,
                About = d.About
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new KeyNotFoundException($"Doctor with id {doctorId} was not found.");

        return dto;
    }
}
