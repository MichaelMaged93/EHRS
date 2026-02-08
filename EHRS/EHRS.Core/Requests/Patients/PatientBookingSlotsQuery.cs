namespace EHRS.Core.Requests.Patients;

public sealed class PatientBookingSlotsQuery
{
    public int DoctorId { get; set; }
    public DateOnly Date { get; set; }
}
