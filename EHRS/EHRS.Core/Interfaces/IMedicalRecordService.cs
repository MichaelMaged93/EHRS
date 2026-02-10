using EHRS.Core.DTOs.MedicalRecords;
using EHRS.Core.Requests.MedicalRecords;

namespace EHRS.Core.Interfaces
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDetailsDto> CreateAsync(
            int doctorId,
            CreateMedicalRecordRequest request,
            CancellationToken ct);

        Task UploadPrescriptionAsync(
            int recordId,
            Stream fileStream,
            string fileName,
            string webRootPath,
            CancellationToken ct);

        Task UploadRadiologyAsync(
            int recordId,
            Stream fileStream,
            string fileName,
            string webRootPath,
            CancellationToken ct);
    }
}
