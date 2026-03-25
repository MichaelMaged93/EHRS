using System.Collections.Generic;

namespace EHRS.Core.DTOs.DoctorPatients
{
    public class PatientSurgeriesDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = default!;

        public string? BloodType { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? WeightKg { get; set; }
        public int? Age { get; set; }

        public List<string> ChronicDiseases { get; set; } = new();
        public List<string> Allergies { get; set; } = new();

        public List<SurgeryForDoctorDto> Surgeries { get; set; } = new();
    }
}