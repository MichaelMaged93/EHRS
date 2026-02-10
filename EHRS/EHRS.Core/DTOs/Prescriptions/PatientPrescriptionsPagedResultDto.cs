namespace EHRS.Core.DTOs.Prescriptions;

public sealed class PatientPrescriptionsPagedResultDto
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public List<PatientPrescriptionCardDto> Items { get; set; } = new();
}
