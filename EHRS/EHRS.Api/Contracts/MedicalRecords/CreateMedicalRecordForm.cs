using Microsoft.AspNetCore.Http;

namespace EHRS.Api.Contracts.MedicalRecords
{
    public sealed class CreateMedicalRecordForm
    {
        public int PatientId { get; set; }
        public int AppointmentId { get; set; }

        public DateTime? RecordDateTime { get; set; }

        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? ClinicalNotes { get; set; }
        public string? Treatment { get; set; }
        public string? Radiology { get; set; }

        // ✅ الروشتة (اختياري)
        public IFormFile? File { get; set; }
    }
}
