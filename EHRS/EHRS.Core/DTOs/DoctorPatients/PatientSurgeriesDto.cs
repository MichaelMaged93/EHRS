using System.Collections.Generic;

namespace EHRS.Core.DTOs.DoctorPatients
{
    public class PatientSurgeriesDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = default!;
        public List<SurgeryForDoctorDto> Surgeries { get; set; } = new();
    }
}