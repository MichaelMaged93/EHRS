using EHRS.Core.DTOs.Doctors;
using EHRS.Core.Interfaces;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Services;

public class DoctorService : IDoctorService
{
    private readonly EHRSContext _db;

    public DoctorService(EHRSContext db) => _db = db;

    public async Task<List<DoctorResponse>> GetAllAsync()
    {
        var doctors = await _db.Doctors
            .AsNoTracking()
            .OrderBy(d => d.DoctorId)
            .ToListAsync();

        return doctors.Select(MapToResponse).ToList();
    }

    public async Task<DoctorResponse?> GetByIdAsync(int doctorId)
    {
        var doctor = await _db.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

        return doctor is null ? null : MapToResponse(doctor);
    }

    public async Task<int> CreateAsync(CreateDoctorRequest request)
    {
        var entity = new Doctor
        {
            FullName = request.FullName,
            Gender = request.Gender,
            BirthDate = request.BirthDate,
            Email = request.Email,
            ContactNumber = request.ContactNumber,
            Specialization = request.Specialization,
            Salary = request.Salary,
            ProfilePicture = request.ProfilePicture,
            Certificates = request.Certificates,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _db.Doctors.Add(entity);
        await _db.SaveChangesAsync();

        return entity.DoctorId;
    }

    public async Task<bool> UpdateAsync(int doctorId, UpdateDoctorRequest request)
    {
        var entity = await _db.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        if (entity is null) return false;

        entity.FullName = request.FullName;
        entity.Gender = request.Gender;
        entity.BirthDate = request.BirthDate;
        entity.Email = request.Email;
        entity.ContactNumber = request.ContactNumber;
        entity.Specialization = request.Specialization;
        entity.Salary = request.Salary;
        entity.ProfilePicture = request.ProfilePicture;
        entity.Certificates = request.Certificates;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int doctorId)
    {
        var entity = await _db.Doctors.FirstOrDefaultAsync(d => d.DoctorId == doctorId);
        if (entity is null) return false;

        _db.Doctors.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    private static DoctorResponse MapToResponse(Doctor d) => new()
    {
        DoctorId = d.DoctorId,
        FullName = d.FullName,
        Gender = d.Gender,
        BirthDate = d.BirthDate,
        Email = d.Email,
        ContactNumber = d.ContactNumber,
        Specialization = d.Specialization,
        Salary = d.Salary,
        ProfilePicture = d.ProfilePicture,
        CreatedAt = d.CreatedAt,
        UpdatedAt = d.UpdatedAt,
        HasCertificates = d.Certificates is { Length: > 0 }
    };
}
