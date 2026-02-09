namespace EHRS.Core.Requests.Patients;

public sealed class UpdateSurgeryRequest
{
    public string? SurgeryType { get; set; }
    public DateOnly? SurgeryDate { get; set; }
    public string? Notes { get; set; }
}
