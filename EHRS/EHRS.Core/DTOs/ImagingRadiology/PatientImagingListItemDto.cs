namespace EHRS.Core.DTOs.ImagingRadiology;

public sealed class PatientImagingListItemDto
{
    public int RecordId { get; set; }
    public DateTime RecordDateTime { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? RadiologyPath { get; set; }
}
