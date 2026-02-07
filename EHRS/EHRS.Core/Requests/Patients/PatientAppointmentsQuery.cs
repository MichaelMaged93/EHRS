namespace EHRS.Core.Requests.Patients;

public sealed class PatientAppointmentsQuery
{
    // Default: first page
    public int Page { get; set; } = 1;

    // Default: 10 cards
    public int PageSize { get; set; } = 10;
}
