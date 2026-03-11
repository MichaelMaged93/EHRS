using System;
using System.Collections.Generic;

namespace EHRS.Core.DTOs.DoctorPatients
{
    public class PatientMedicalHistoryDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = default!;
        public string BloodType { get; set; } = default!;
        public List<MedicalRecordForDoctorDto> MedicalRecords { get; set; } = new();
    }
}