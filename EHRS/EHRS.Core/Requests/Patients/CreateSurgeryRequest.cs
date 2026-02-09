namespace EHRS.Core.Requests.Patients;

public sealed class CreateSurgeryRequest
{
    public string SurgeryType { get; set; } = string.Empty;
    public DateOnly SurgeryDate { get; set; }
    public string? Notes { get; set; }
}
