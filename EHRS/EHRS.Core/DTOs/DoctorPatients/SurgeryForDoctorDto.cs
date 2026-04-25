namespace EHRS.Core.DTOs.DoctorPatients
{
    public class SurgeryForDoctorDto
    {
        public int SurgeryId { get; set; }
        public string SurgeryType { get; set; } = default!;
        public DateTime SurgeryDate { get; set; }

        public int? DoctorId { get; set; }

        // 👇 الجديد
        public string? DoctorName { get; set; }
        public string? DoctorSpecialization { get; set; }
        public string? DoctorImageUrl { get; set; }

        public string? Notes { get; set; }
    }
}