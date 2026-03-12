using System;

namespace EHRS.Core.DTOs.DoctorPatients
{
    public class SurgeryForDoctorDto
    {
        public int SurgeryId { get; set; }
        public string SurgeryType { get; set; } = default!;
        public DateTime SurgeryDate { get; set; }
        public int? DoctorId { get; set; } // nullable لتجنب أي مشكلة عند القراءة
        public string? Notes { get; set; }
    }
}