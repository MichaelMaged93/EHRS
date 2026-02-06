using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence.Entities;

[Table("MedicalRecord")]
[Index("DoctorId", "RecordDateTime", Name = "IX_MedicalRecord_DoctorDate", IsDescending = new[] { false, true })]
[Index("PatientId", "RecordDateTime", Name = "IX_MedicalRecord_PatientDate", IsDescending = new[] { false, true })]
[Index("AppointmentId", Name = "UX_MedicalRecord_Appointment", IsUnique = true)]
public partial class MedicalRecord
{
    [Key]
    public int RecordId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int AppointmentId { get; set; }

    [Precision(0)]
    public DateTime RecordDateTime { get; set; }

    [StringLength(300)]
    public string? ChiefComplaint { get; set; }

    [StringLength(500)]
    public string? Diagnosis { get; set; }

    public string? ClinicalNotes { get; set; }

    [StringLength(500)]
    public string? Treatment { get; set; }

    [StringLength(300)]
    public string? Radiology { get; set; }

    [StringLength(300)]
    public string? PrescriptionImagePath { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("MedicalRecord")]
    public virtual Appointment Appointment { get; set; } = null!;

    [ForeignKey("DoctorId")]
    [InverseProperty("MedicalRecords")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("MedicalRecords")]
    public virtual Patient Patient { get; set; } = null!;
}
