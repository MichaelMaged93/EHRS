public sealed class AppointmentListItemDto
{
    public int AppointmentId { get; init; }

    public int PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public string? Gender { get; init; }
    public DateOnly? BirthDate { get; init; }
    public int? Age { get; init; }
    public string? BloodType { get; init; }
    public decimal? HeightCm { get; init; }
    public decimal? WeightKg { get; init; }
    public string? Address { get; init; }
    public string? ProfilePicture { get; init; }

    public DateTime AppointmentDateTime { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}