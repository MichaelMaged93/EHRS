using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.Persistence.Entities;

public partial class UserCredential
{
    public int CredentialId { get; set; }

    public string Role { get; set; } = null!;

    public int? PatientId { get; set; }

    public int? DoctorId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Patient? Patient { get; set; }
}
