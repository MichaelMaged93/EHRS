namespace EHRS.Core.Common;

public enum PatientAuthError
{
    None = 0,
    PasswordsDoNotMatch = 1,
    EmailAlreadyExists = 2,
    NationalIdAlreadyExists = 3,
    InvalidCredentials = 4
}
