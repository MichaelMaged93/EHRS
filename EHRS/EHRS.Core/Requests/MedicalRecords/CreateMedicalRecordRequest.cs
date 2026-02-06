using System.ComponentModel.DataAnnotations;

namespace EHRS.Core.Requests.MedicalRecords
{
    public sealed class CreateMedicalRecordRequest
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        public DateTime? RecordDateTime { get; set; }

        [MaxLength(300)]
        public string? ChiefComplaint { get; set; }

        [MaxLength(500)]
        public string? Diagnosis { get; set; }

        public string? ClinicalNotes { get; set; }

        [MaxLength(500)]
        public string? Treatment { get; set; }

        [MaxLength(300)]
        public string? Radiology { get; set; }

        [MaxLength(300)]
        public string? PrescriptionImagePath { get; set; }
    }
}
