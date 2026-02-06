namespace EHRS.Core.Requests.MedicalRecords
{
    public sealed class MedicalRecordQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? Search { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
