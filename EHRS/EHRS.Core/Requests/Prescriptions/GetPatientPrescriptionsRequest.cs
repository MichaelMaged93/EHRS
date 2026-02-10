namespace EHRS.Core.Requests.Prescriptions;

public sealed class GetPatientPrescriptionsRequest
{
    public string Tab { get; set; } = "active"; // active | past

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
