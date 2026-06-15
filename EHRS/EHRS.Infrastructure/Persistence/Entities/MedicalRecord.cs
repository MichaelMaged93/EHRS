using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.Persistence.Entities;

public partial class MedicalRecord
{
    public int RecordId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int AppointmentId { get; set; }

    public DateTime RecordDateTime { get; set; }

    public string? ChiefComplaint { get; set; }

    public string? Diagnosis { get; set; }

    public string? ClinicalNotes { get; set; }

    public string? Treatment { get; set; }

    public string? Radiology { get; set; }

    public string? PrescriptionImagePath { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
