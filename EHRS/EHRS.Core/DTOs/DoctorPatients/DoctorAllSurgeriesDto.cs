public class DoctorAllSurgeriesDto
{
    public int SurgeryId { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = null!;
    public string? PatientImageUrl { get; set; } // ✅ جديد
    public string SurgeryType { get; set; } = null!;
    public DateTime SurgeryDate { get; set; }
    public string? Notes { get; set; }
}