namespace EHRS.Core.DTOs.Patients;

public sealed class PatientSurgeryDto
{
    public int SurgeryId { get; set; }
    public string SurgeryType { get; set; } = string.Empty;
    public DateOnly SurgeryDate { get; set; }
    public string? Notes { get; set; }
}
