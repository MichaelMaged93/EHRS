using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence.Entities;

[Table("Patient")]
[Index("Ssn", Name = "UQ_Patient_SSN", IsUnique = true)]
public partial class Patient
{
    [Key]
    public int PatientId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [StringLength(10)]
    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    [StringLength(120)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? ContactNumber { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(5)]
    public string? BloodType { get; set; }

    public short? HeightCm { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? WeightKg { get; set; }

    [StringLength(300)]
    public string? ProfilePicture { get; set; }

    [Column("SSN")]
    [StringLength(20)]
    public string Ssn { get; set; } = null!;

    public string? Diseases { get; set; }

    public string? Allergies { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Patient")]
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    [InverseProperty("Patient")]
    public virtual ICollection<SensorDatum> SensorData { get; set; } = new List<SensorDatum>();

    [InverseProperty("Patient")]
    public virtual ICollection<SurgeryHistory> SurgeryHistories { get; set; } = new List<SurgeryHistory>();
}
