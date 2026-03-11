namespace EHRS.Core.DTOs.Prescriptions;

public sealed class PatientPrescriptionCardDto
{
    public int RecordId { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string? DoctorSpecialization { get; set; }

    public DateTime PrescriptionDate { get; set; }

    public string? ChiefComplaint { get; set; }

    public string? Diagnosis { get; set; }

    public string? Treatment { get; set; }

    public string PrescriptionPath { get; set; } = string.Empty;

    public string DownloadUrl { get; set; } = string.Empty;
}
