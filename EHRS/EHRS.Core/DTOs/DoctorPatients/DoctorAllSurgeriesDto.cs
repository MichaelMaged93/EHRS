using System;

namespace EHRS.Core.DTOs.DoctorPatients
{
    public class DoctorAllSurgeriesDto
    {
        public int SurgeryId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = default!;
        public string SurgeryType { get; set; } = default!;
        public DateTime SurgeryDate { get; set; }
        public string? Notes { get; set; }
    }
}