namespace EHRS.Core.DTOs.Patients;

public sealed class PatientDashboardDto
{
    public int PatientId { get; set; }
    public string FullName { get; set; } = string.Empty;

    // From Patient table
    public short? HeightCm { get; set; }
    public decimal? WeightKg { get; set; }
    public string? BloodType { get; set; }
    public decimal? Bmi { get; set; }

    // From SensorDatum (latest)
    public PatientVitalSignsDto VitalSigns { get; set; } = new();

    // From Appointments (next upcoming)
    public PatientUpcomingAppointmentDto? UpcomingAppointment { get; set; }

    // From MedicalRecords (recent)
    public List<PatientRecentVisitDto> RecentVisits { get; set; } = new();
}

public sealed class PatientVitalSignsDto
{
    public DateTime? Timestamp { get; set; }

    public decimal? Temperature { get; set; }
    public short? HeartRate { get; set; }
    public decimal? PressureHeart { get; set; }
    public decimal? SpO2 { get; set; }
    public string? ActivityLevel { get; set; }
}

public sealed class PatientUpcomingAppointmentDto
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDateTime { get; set; }

    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;

    public byte? Status { get; set; }
    public string? ReasonForVisit { get; set; }
}

public sealed class PatientRecentVisitDto
{
    public int RecordId { get; set; }
    public DateTime RecordDateTime { get; set; }

    public string? Diagnosis { get; set; }
    public string? Notes { get; set; } // ClinicalNotes
}
