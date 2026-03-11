using System;

namespace EHRS.Core.DTOs.DoctorPatients
{
    public class MedicalRecordForDoctorDto
    {
        public int RecordId { get; set; }
        public string DoctorName { get; set; } = default!;
        public string Diagnosis { get; set; } = default!;
        public string Treatment { get; set; } = default!;
        public DateTime RecordDateTime { get; set; }
    }
}