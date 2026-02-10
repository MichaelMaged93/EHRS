namespace EHRS.Core.DTOs.Prescriptions;

public sealed class PrescriptionFileRefDto
{
    public int RecordId { get; set; }

    // Stored path in DB (relative or absolute)
    public string PrescriptionPath { get; set; } = string.Empty;
}
