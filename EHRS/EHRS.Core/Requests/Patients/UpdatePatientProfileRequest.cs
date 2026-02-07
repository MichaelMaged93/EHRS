namespace EHRS.Core.Requests.Patients;

public sealed class UpdatePatientProfileRequest
{
    public string? FullName { get; set; }
    public string? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }

    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }

    public string? BloodType { get; set; }
    public short? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }

    public string? Ssn { get; set; }
}
