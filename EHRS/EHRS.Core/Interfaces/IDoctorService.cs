using EHRS.Core.DTOs.Doctors;

namespace EHRS.Core.Interfaces;

public interface IDoctorService
{
    Task<List<DoctorResponse>> GetAllAsync();
    Task<DoctorResponse?> GetByIdAsync(int doctorId);
    Task<int> CreateAsync(CreateDoctorRequest request);
    Task<bool> UpdateAsync(int doctorId, UpdateDoctorRequest request);
    Task<bool> DeleteAsync(int doctorId);
}
