using System;
using System.Collections.Generic;

namespace EHRS.Infrastructure.Persistence.Entities;

public partial class SensorDatum
{
    public long SensorDataId { get; set; }

    public int PatientId { get; set; }

    public DateTime Timestamp { get; set; }

    public short? HeartRate { get; set; }

    public decimal? SpO2 { get; set; }

    public decimal? Temperature { get; set; }

    public string? ActivityLevel { get; set; }

    public decimal? PressureHeart { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
