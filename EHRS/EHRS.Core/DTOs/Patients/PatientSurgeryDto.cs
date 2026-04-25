namespace EHRS.Core.DTOs.Patients;

public sealed class PatientSurgeryDto
{
    public int SurgeryId { get; set; }

    public int DoctorId { get; set; }

    public string SurgeryType { get; set; } = string.Empty;

    public DateOnly SurgeryDate { get; set; }

    public string? Notes { get; set; }

    // 👇 الجديد
    public DoctorBasicInfoDto Doctor { get; set; } = new();
}

public sealed class DoctorBasicInfoDto
{
    public int DoctorId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Specialty { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
}