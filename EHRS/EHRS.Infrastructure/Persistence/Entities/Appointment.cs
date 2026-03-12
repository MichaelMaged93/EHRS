using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence.Entities;

[Table("Appointment")]
[Index("DoctorId", "AppointmentDateTime", Name = "IX_Appointment_DoctorDate", IsDescending = new[] { false, true })]
[Index("PatientId", "AppointmentDateTime", Name = "IX_Appointment_PatientDate", IsDescending = new[] { false, true })]
public partial class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    [Precision(0)]
    public DateTime AppointmentDateTime { get; set; }

    public byte Status { get; set; }

    [StringLength(200)]
    public string? ReasonForVisit { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    public bool IsCancelled { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("Appointments")]
    public virtual Doctor Doctor { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual MedicalRecord? MedicalRecord { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual Patient Patient { get; set; } = null!;
}
