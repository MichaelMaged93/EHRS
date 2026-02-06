using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Persistence.Entities;

[Index("PatientId", "Timestamp", Name = "IX_SensorData_PatientTime", IsDescending = new[] { false, true })]
public partial class SensorDatum
{
    [Key]
    public long SensorDataId { get; set; }

    public int PatientId { get; set; }

    [Precision(0)]
    public DateTime Timestamp { get; set; }

    public short? HeartRate { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? SpO2 { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? Temperature { get; set; }

    [StringLength(50)]
    public string? ActivityLevel { get; set; }

    [Column("pressure_heart", TypeName = "decimal(5, 2)")]
    public decimal? PressureHeart { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("SensorData")]
    public virtual Patient Patient { get; set; } = null!;
}
