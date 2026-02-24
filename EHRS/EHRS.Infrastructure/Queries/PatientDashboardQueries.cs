using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using EHRS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class PatientDashboardQueries : IPatientDashboardQueries
{
    private readonly EHRSContext _db;

    public PatientDashboardQueries(EHRSContext db)
    {
        _db = db;
    }

    public async Task<PatientDashboardDto?> GetAsync(int patientId, CancellationToken ct = default)
    {
        var patient = await _db.Patients.FindAsync(new object?[] { patientId }, ct);
        if (patient is null) return null;

        // Latest sensor record
        var latestSensor = await _db.SensorData
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.Timestamp)
            .FirstOrDefaultAsync(ct);

        // Upcoming appointment (nearest)
        var now = DateTime.UtcNow;

        var upcoming = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId
                        && !a.IsCancelled
                        && a.AppointmentDateTime >= now)
            .OrderBy(a => a.AppointmentDateTime)
            .Select(a => new PatientUpcomingAppointmentDto
            {
                AppointmentId = a.AppointmentId,
                AppointmentDateTime = a.AppointmentDateTime,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.FullName,

                //  Unified display status: waiting | completed | cancelled
                Status = MapStatusToText(a.Status, a.IsCancelled),

                ReasonForVisit = a.ReasonForVisit
            })
            .FirstOrDefaultAsync(ct);

        // Recent visits
        var recentVisits = await _db.MedicalRecords
            .AsNoTracking()
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.RecordDateTime)
            .Take(5)
            .Select(r => new PatientRecentVisitDto
            {
                RecordId = r.RecordId,
                RecordDateTime = r.RecordDateTime,
                Diagnosis = r.Diagnosis,
                Notes = r.ClinicalNotes
            })
            .ToListAsync(ct);

        var bmi = CalculateBmi(patient.HeightCm, patient.WeightKg);

        return new PatientDashboardDto
        {
            PatientId = patient.PatientId,
            FullName = patient.FullName,

            HeightCm = patient.HeightCm,
            WeightKg = patient.WeightKg,
            BloodType = patient.BloodType,
            Bmi = bmi,

            VitalSigns = new PatientVitalSignsDto
            {
                Timestamp = latestSensor?.Timestamp,
                Temperature = latestSensor?.Temperature,
                HeartRate = latestSensor?.HeartRate,
                PressureHeart = latestSensor?.PressureHeart,
                SpO2 = latestSensor?.SpO2,
                ActivityLevel = latestSensor?.ActivityLevel
            },

            UpcomingAppointment = upcoming,
            RecentVisits = recentVisits
        };
    }

    private static string MapStatusToText(byte status, bool isCancelled)
    {
        if (isCancelled) return "cancelled";

        return status switch
        {
            0 => "waiting",
            1 => "completed",
            _ => "unknown"
        };
    }

    private static decimal? CalculateBmi(short? heightCm, decimal? weightKg)
    {
        if (heightCm is null || heightCm.Value <= 0) return null;
        if (weightKg is null || weightKg.Value <= 0) return null;

        var heightM = (decimal)heightCm.Value / 100m;
        var bmi = weightKg.Value / (heightM * heightM);

        return Math.Round(bmi, 1);
    }
}
