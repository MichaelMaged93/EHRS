namespace EHRS.Core.DTOs.ImagingRadiology;

public sealed class PatientImagingListItemDto
{
    public int RecordId { get; set; }
    public DateTime RecordDateTime { get; set; }
    public string? ClinicalNotes { get; set; }

    // Radiology في MedicalRecord هنعتبره path/URL للصورة
    public string? RadiologyPath { get; set; }
}
