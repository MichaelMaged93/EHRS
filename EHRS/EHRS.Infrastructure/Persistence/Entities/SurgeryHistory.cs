using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.Persistence.Entities;

public partial class SurgeryHistory
{
    public int SurgeryId { get; set; }

    public int PatientId { get; set; }

    public string SurgeryType { get; set; } = null!;

    public DateOnly SurgeryDate { get; set; }

    public string? Notes { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
