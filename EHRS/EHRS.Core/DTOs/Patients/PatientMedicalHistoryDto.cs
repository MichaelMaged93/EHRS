namespace EHRS.Core.DTOs.Patients;

public sealed class PatientMedicalHistoryDto
{
    public List<string> ChronicDiseases { get; set; } = new();
    public List<string> Allergies { get; set; } = new();
    public List<PatientSurgeryDto> Surgeries { get; set; } = new();
}
