
namespace EHRS.Infrastructure.Queries
{
    public class PatientInfoForMedicalRecordDto
    {
        public decimal? WeightKg { get; internal set; }
        public short? HeightCm { get; internal set; }
        public string? BloodType { get; internal set; }
        public DateOnly? BirthDate { get; internal set; }
        public string? Gender { get; internal set; }
        public string FullName { get; internal set; }
        public int PatientId { get; internal set; }
    }
}