using EHRS.Core.DTOs.Patients;

namespace EHRS.Core.DTOs.MedicalRecords
{
    public sealed class MedicalRecordDetailsDto
    {
        public int RecordId { get; set; }
        
        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }

        public DateTime RecordDateTime { get; set; }

        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? ClinicalNotes { get; set; }
        public string? Treatment { get; set; }

        public string? Radiology { get; set; }
        public string? PrescriptionImagePath { get; set; }

        // ✅ الوحيد الموجود
        public PatientMedicalViewDto Patient { get; set; }
    }
}