using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;

namespace EHRS.Core.Abstractions.Queries;

public interface IPatientBookingQueries
{
    //  Booking (Create) بالتاريخ فقط
    Task<PatientBookingDto> CreateAsync(
        int patientId,
        CreatePatientBookingRequest request,
        CancellationToken ct);

    //  Dropdowns لل UI
    Task<IReadOnlyList<string>> GetAreasAsync(CancellationToken ct);

    Task<IReadOnlyList<string>> GetSpecialtiesAsync(string area, CancellationToken ct);

    Task<IReadOnlyList<PatientBookingDoctorDto>> GetDoctorsAsync(string area, string specialty, CancellationToken ct);
}
