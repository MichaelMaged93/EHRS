using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.Persistence.Entities;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Gender { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? Email { get; set; }

    public string? ContactNumber { get; set; }

    public string? Specialization { get; set; }

    public decimal? Salary { get; set; }

    public string? ProfilePicture { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Certificates { get; set; }

    public string? AffiliatedHospital { get; set; }

    public string? About { get; set; }

    public string MedicalLicense { get; set; } = null!;

    public string? Area { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
