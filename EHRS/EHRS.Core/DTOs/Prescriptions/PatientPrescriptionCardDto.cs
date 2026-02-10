namespace EHRS.Core.DTOs.Prescriptions;

public sealed class PatientPrescriptionCardDto
{
    public int RecordId { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string? DoctorSpecialization { get; set; }

    public DateTime PrescriptionDate { get; set; }

    // The stored path in DB (useful for debugging / internal use)
    public string PrescriptionPath { get; set; } = string.Empty;

    // A relative URL for viewing/downloading the file via API
    public string DownloadUrl { get; set; } = string.Empty;
}
