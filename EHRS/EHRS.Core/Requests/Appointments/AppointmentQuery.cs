namespace EHRS.Core.Requests.Appointments;

public sealed class AppointmentQuery
{
    public string Scope { get; init; } = "upcoming"; // upcoming | past
    public string? Status { get; init; }            // waiting | completed | cancelled
    public string? Search { get; init; }

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
