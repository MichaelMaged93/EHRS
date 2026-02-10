namespace EHRS.Core.Requests.ImagingRadiology;

public sealed class GetPatientImagingRequest
{
    public int Page { get; set; } = 1;       // متوافق مع PagedResult (Page)
    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }
}
