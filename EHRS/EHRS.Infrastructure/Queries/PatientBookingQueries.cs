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

    public async Task<IReadOnlyList<string>> GetAreasAsync(CancellationToken ct)
    {
        return await _context.Doctors
            .Where(d => d.Area != null && d.Area != "")
            .Select(d => d.Area!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetSpecialtiesAsync(string area, CancellationToken ct)
    {
        area = area?.Trim() ?? "";

        return await _context.Doctors
            .Where(d => d.Area == area && d.Specialization != null && d.Specialization != "")
            .Select(d => d.Specialization!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<PatientBookingDoctorDto>> GetDoctorsAsync(
        string area,
        string specialty,
        CancellationToken ct)
    {
        area = area?.Trim() ?? "";
        specialty = specialty?.Trim() ?? "";

        return await _context.Doctors
            .Where(d => d.Area == area && d.Specialization == specialty)
            .OrderBy(d => d.FullName)
            .Select(d => new PatientBookingDoctorDto
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                Specialization = d.Specialization,
                Area = d.Area
            })
            .ToListAsync(ct);
    }

    public async Task<PatientBookingDto> CreateAsync(
        int patientId,
        CreatePatientBookingRequest request,
        CancellationToken ct)
    {
        if (request.DoctorId <= 0)
            throw new ArgumentException("DoctorId is required.");

        // ✅ FIXED
        if (request.AppointmentDate < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("AppointmentDate must be today or in the future.");

        var patientExists = await _context.Patients
            .AnyAsync(p => p.PatientId == patientId, ct);

        if (!patientExists)
            throw new InvalidOperationException("Patient not found.");

        var doctor = await _context.Doctors
            .Where(d => d.DoctorId == request.DoctorId)
            .Select(d => new { d.DoctorId, d.Area, d.Specialization })
            .FirstOrDefaultAsync(ct);

        if (doctor is null)
            throw new InvalidOperationException("Doctor not found.");

        var appointmentDateTime = request.AppointmentDate
            .ToDateTime(new TimeOnly(0, 0));

        var dayStart = appointmentDateTime.Date;
        var dayEnd = dayStart.AddDays(1);

        var duplicate = await _context.Appointments.AnyAsync(a =>
            a.PatientId == patientId &&
            a.DoctorId == request.DoctorId &&
            !a.IsCancelled &&
            a.AppointmentDateTime >= dayStart &&
            a.AppointmentDateTime < dayEnd, ct);

        if (duplicate)
            throw new InvalidOperationException("Already booked for this day.");

        var entity = new Appointment
        {
            PatientId = patientId,
            DoctorId = request.DoctorId,
            AppointmentDateTime = appointmentDateTime,
            ReasonForVisit = request.ReasonForVisit,
            Status = 1,
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