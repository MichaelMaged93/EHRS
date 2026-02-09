namespace EHRS.Core.Requests.Patients;

public sealed class UpdatePatientMedicalHistoryRequest
{
    public List<string>? ChronicDiseases { get; set; }
    public List<string>? Allergies { get; set; }
}
