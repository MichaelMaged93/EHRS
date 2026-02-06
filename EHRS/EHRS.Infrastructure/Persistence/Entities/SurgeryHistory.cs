using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence.Entities;

[Table("SurgeryHistory")]
public partial class SurgeryHistory
{
    [Key]
    public int SurgeryId { get; set; }

    public int PatientId { get; set; }

    [StringLength(150)]
    public string SurgeryType { get; set; } = null!;

    public DateOnly SurgeryDate { get; set; }

    [StringLength(300)]
    public string? Notes { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("SurgeryHistories")]
    public virtual Patient Patient { get; set; } = null!;
}
