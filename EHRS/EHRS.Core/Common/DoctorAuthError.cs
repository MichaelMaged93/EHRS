namespace EHRS.Core.Common;

public enum DoctorAuthError
{
    None = 0,
    PasswordsDoNotMatch = 1,
    EmailAlreadyExists = 2,
    MedicalLicenseAlreadyExists = 3,
    PendingApproval = 4,
    Rejected = 5,
    InvalidCredentials = 6
}
