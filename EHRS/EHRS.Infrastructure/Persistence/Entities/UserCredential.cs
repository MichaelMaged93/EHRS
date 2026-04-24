using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence.Entities;

[Table("UserCredential")]
public partial class UserCredential
{
    [Key]
    public int CredentialId { get; set; }

    [StringLength(20)]
    public string Role { get; set; } = null!;

    public int? PatientId { get; set; }

    public int? DoctorId { get; set; }

    [StringLength(500)]
    public string PasswordHash { get; set; } = null!;

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("UserCredential")]
    public virtual Doctor? Doctor { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("UserCredential")]
    public virtual Patient? Patient { get; set; }
    public string? ResetToken { get; set; }

    public DateTime? ResetTokenExpiry { get; set; }

}
