using EHRS.Core.Common;
using EHRS.Core.DTOs.MedicalRecords;
using EHRS.Core.Requests.MedicalRecords;

namespace EHRS.Core.Abstractions.Queries
{
    public interface IMedicalRecordQueries
    {
        Task<PagedResult<MedicalRecordListItemDto>> GetDoctorMedicalRecordsAsync(
            int doctorId,
            MedicalRecordQuery query,
            CancellationToken ct);

        Task<MedicalRecordDetailsDto?> GetByIdAsync(int recordId, CancellationToken ct);

        Task<MedicalRecordDetailsDto?> GetByAppointmentAsync(int appointmentId, CancellationToken ct);
        Task<List<MedicalRecordDetailsDto>> GetByPatientAsync(
            int doctorId,
            int patientId,
            CancellationToken ct);
    }
}