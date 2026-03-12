using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.TempEntities;

public partial class SurgeryHistory
{
    public int SurgeryId { get; set; }

    public int PatientId { get; set; }

    public string SurgeryType { get; set; } = null!;

    public DateOnly SurgeryDate { get; set; }

    public string? Notes { get; set; }

    public int? DoctorId { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
