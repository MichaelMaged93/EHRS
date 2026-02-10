using EHRS.Core.Common;
using EHRS.Core.DTOs.ImagingRadiology;
using EHRS.Core.Requests.ImagingRadiology;

namespace EHRS.Core.Interfaces;

public interface IPatientImagingQueries
{
    Task<PagedResult<PatientImagingListItemDto>> GetPatientImagingAsync(
        int patientId,
        GetPatientImagingRequest request);

    Task<PatientImagingListItemDto?> GetPatientImagingByRecordIdAsync(
        int patientId,
        int recordId);
}
