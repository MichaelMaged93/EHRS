namespace EHRS.Core.DTOs.Patients;

public sealed class PatientBookingDto
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public string? FullName { get; set; }

    public string? Specialization { get; set; }

    public string? Area { get; set; }

    public string? ProfilePicture { get; set; }

    public string? ContactNumber { get; set; }

    public decimal? Salary { get; set; }

    public int? Age { get; set; }

    public string? About { get; set; }

    public string? AffiliatedHospital { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public byte Status { get; set; }

    public string? ReasonForVisit { get; set; }

    public bool IsCancelled { get; set; }
}