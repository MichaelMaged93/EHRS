namespace EHRS.Core.DTOs.Prescriptions;

public sealed class PatientPrescriptionCardDto
{
    public int RecordId { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string? DoctorSpecialization { get; set; }

    public DateTime PrescriptionDate { get; set; }

    //  Medical Record details
    public string? ChiefComplaint { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    // Stored path in DB
    public string PrescriptionPath { get; set; } = string.Empty;

    // API endpoint for downloading the prescription
    public string DownloadUrl { get; set; } = string.Empty;
}
