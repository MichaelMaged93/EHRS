namespace EHRS.Core.DTOs.MedicalRecords
{
    public sealed class MedicalRecordDetailsDto
    {
        public int RecordId { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";

        public int DoctorId { get; set; }
        public int AppointmentId { get; set; }

        public DateTime RecordDateTime { get; set; }

        public string? ChiefComplaint { get; set; }
        public string? Diagnosis { get; set; }
        public string? ClinicalNotes { get; set; }
        public string? Treatment { get; set; }

        //  لازم عشان Error بتاع Radiology
        public string? Radiology { get; set; }

        //  لينك/مسار الصورة
        public string? PrescriptionImagePath { get; set; }
    }
}
