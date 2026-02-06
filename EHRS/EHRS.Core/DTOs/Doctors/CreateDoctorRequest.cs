using System.ComponentModel.DataAnnotations;

namespace EHRS.Core.DTOs.Doctors;

public class CreateDoctorRequest
{
    [Required, StringLength(100)]
    public string FullName { get; set; } = default!;

    [StringLength(10)]
    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    [StringLength(120)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? ContactNumber { get; set; }

    [StringLength(100)]
    public string? Specialization { get; set; }

    [Range(0, 9999999999)]
    public decimal? Salary { get; set; }

    [StringLength(300)]
    public string? ProfilePicture { get; set; }

    public byte[]? Certificates { get; set; } // Swagger هيرسلها Base64
}
