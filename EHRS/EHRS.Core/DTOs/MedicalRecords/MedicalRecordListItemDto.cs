namespace EHRS.Core.DTOs.MedicalRecords
{
    public sealed class MedicalRecordListItemDto
    {
        public int RecordId { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";

        public DateTime RecordDateTime { get; set; }

        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
    }
}
