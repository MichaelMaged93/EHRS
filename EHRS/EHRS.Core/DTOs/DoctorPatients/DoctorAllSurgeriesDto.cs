public class DoctorAllSurgeriesDto
{
    public int SurgeryId { get; set; }

    public int PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public string? PatientImageUrl { get; set; }

    // ➜ إضافات جديدة من بيانات المريض
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public int? Age { get; set; }
    public string? BloodType { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public string? Address { get; set; }

    public string SurgeryType { get; set; } = null!;
    public DateTime SurgeryDate { get; set; }
    public string? Notes { get; set; }
}