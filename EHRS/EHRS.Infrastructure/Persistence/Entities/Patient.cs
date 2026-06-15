using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.Persistence.Entities;

public partial class Patient
{
    public int PatientId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Email { get; set; }

    public string? ContactNumber { get; set; }

    public string? Address { get; set; }

    public string? BloodType { get; set; }

    public short? HeightCm { get; set; }

    public decimal? WeightKg { get; set; }

    public string? ProfilePicture { get; set; }

    public string Ssn { get; set; } = null!;

    public string? Diseases { get; set; }

    public string? Allergies { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<SensorDatum> SensorData { get; set; } = new List<SensorDatum>();

    public virtual ICollection<SurgeryHistory> SurgeryHistories { get; set; } = new List<SurgeryHistory>();

    public virtual UserCredential? UserCredential { get; set; }
}
