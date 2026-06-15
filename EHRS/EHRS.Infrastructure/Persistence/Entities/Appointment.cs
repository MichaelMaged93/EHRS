using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.Persistence.Entities;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    public byte Status { get; set; }

    public string? ReasonForVisit { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsCancelled { get; set; }

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual MedicalRecord? MedicalRecord { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
