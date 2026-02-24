using EHRS.Core.Abstractions.Queries;
using EHRS.Core.DTOs.Patients;
using EHRS.Core.Requests.Patients;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Infrastructure.Queries;

public sealed class PatientBookingQueries : IPatientBookingQueries
{
    private readonly EHRSContext _context;

    public PatientBookingQueries(EHRSContext context)
    {
        _context = context;
    }

    // =========================
    // UI: Areas
    // =========================
    public async Task<IReadOnlyList<string>> GetAreasAsync(CancellationToken ct)
    {
        var areas = await _context.Doctors
            .Where(d => d.Area != null && d.Area != "")
            .Select(d => d.Area!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

        return areas;
    }

    // =========================
    // UI: Specialties by Area
    // =========================
    public async Task<IReadOnlyList<string>> GetSpecialtiesAsync(string area, CancellationToken ct)
    {
        area = area?.Trim() ?? "";

        var specialties = await _context.Doctors
            .Where(d => d.Area != null && d.Area == area && d.Specialization != null && d.Specialization != "")
            .Select(d => d.Specialization!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

        return specialties;
    }

    // =========================
    // UI: Doctors by Area + Specialty
    // =========================
    public async Task<IReadOnlyList<PatientBookingDoctorDto>> GetDoctorsAsync(string area, string specialty, CancellationToken ct)
    {
        area = area?.Trim() ?? "";
        specialty = specialty?.Trim() ?? "";

        var doctors = await _context.Doctors
            .Where(d =>
                d.Area != null && d.Area == area &&
                d.Specialization != null && d.Specialization == specialty)
            .OrderBy(d => d.FullName)
            .Select(d => new PatientBookingDoctorDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Specialization = d.Specialization,
                Area = d.Area
            })
            .ToListAsync(ct);

        return doctors;
    }

    // =========================
    // POST: Create Booking (Date only)
    // =========================
    public async Task<PatientBookingDto> CreateAsync(
        int patientId,
        CreatePatientBookingRequest request,
        CancellationToken ct)
    {
        if (request.DoctorId <= 0)
            throw new ArgumentException("DoctorId is required.");

        if (request.AppointmentDate < DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("AppointmentDate must be today or in the future.");

        if (string.IsNullOrWhiteSpace(request.Area))
            throw new ArgumentException("Area is required.");

        if (string.IsNullOrWhiteSpace(request.Specialty))
            throw new ArgumentException("Specialty is required.");

        // Ensure Patient exists
        var patientExists = await _context.Patients
            .AnyAsync(p => p.PatientId == patientId, ct);

        if (!patientExists)
            throw new InvalidOperationException("Patient not found.");

        //  Verify doctor matches selected Area + Specialty
        var area = request.Area.Trim();
        var specialty = request.Specialty.Trim();

        var doctor = await _context.Doctors
            .Where(d => d.DoctorId == request.DoctorId)
            .Select(d => new
            {
                d.DoctorId,
                d.Area,
                d.Specialization
            })
            .FirstOrDefaultAsync(ct);

        if (doctor is null)
            throw new InvalidOperationException("Doctor not found.");

        if (string.IsNullOrWhiteSpace(doctor.Area) ||
            !string.Equals(doctor.Area.Trim(), area, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Selected doctor does not belong to the selected area.");

        if (string.IsNullOrWhiteSpace(doctor.Specialization) ||
            !string.Equals(doctor.Specialization.Trim(), specialty, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Selected doctor does not belong to the selected specialty.");

        //  تحويل DateOnly إلى DateTime ثابت (00:00:00)
        var appointmentDateTime = request.AppointmentDate.ToDateTime(new TimeOnly(0, 0));
        var dayStart = appointmentDateTime.Date;
        var dayEnd = appointmentDateTime.Date.AddDays(1);

        //  منع نفس المريض يحجز نفس الدكتور في نفس اليوم
        var duplicateSameDoctorSameDay = await _context.Appointments.AnyAsync(a =>
            a.PatientId == patientId &&
            a.DoctorId == request.DoctorId &&
            !a.IsCancelled &&
            a.AppointmentDateTime >= dayStart &&
            a.AppointmentDateTime < dayEnd, ct);

        if (duplicateSameDoctorSameDay)
            throw new InvalidOperationException("You already have an appointment with this doctor on the selected date.");

        var entity = new Appointment
        {
            PatientId = patientId,
            DoctorId = request.DoctorId,
            AppointmentDateTime = appointmentDateTime,
            ReasonForVisit = request.ReasonForVisit,
            Status = 1, // Scheduled
            IsCancelled = false
        };

        _context.Appointments.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new PatientBookingDto
        {
            AppointmentId = entity.AppointmentId,
            PatientId = entity.PatientId,
            DoctorId = entity.DoctorId,
            AppointmentDate = DateOnly.FromDateTime(entity.AppointmentDateTime),
            Status = entity.Status,
            ReasonForVisit = entity.ReasonForVisit,
            IsCancelled = entity.IsCancelled
        };
    }
}
