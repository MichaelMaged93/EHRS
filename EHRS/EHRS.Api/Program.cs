using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Interfaces;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Queries;
using EHRS.Infrastructure.Queries.Patients;
using EHRS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace EHRS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Controllers & Swagger
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // DbContext
            var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            builder.Services.AddDbContext<EHRSContext>(options =>
                options.UseSqlServer(connStr));

            // Services
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            builder.Services.AddScoped<IDoctorProfileService, DoctorProfileService>();

            // Queries
            builder.Services.AddScoped<IAppointmentQueries, AppointmentQueries>();
            builder.Services.AddScoped<IDashboardQueries, DashboardQueries>();
            builder.Services.AddScoped<IMedicalRecordQueries, MedicalRecordQueries>();
            builder.Services.AddScoped<IDoctorProfileQueries, DoctorProfileQueries>();
            builder.Services.AddScoped<IPatientProfileQueries, PatientProfileQueries>();
            builder.Services.AddScoped<IPatientPrescriptionsQueries, PatientPrescriptionsQueries>();

            // Patient Medical History (Diseases / Allergies)
            builder.Services.AddScoped<IPatientMedicalHistoryQueries, PatientMedicalHistoryQueries>();

            // Patient Dashboard
            builder.Services.AddScoped<IPatientDashboardQueries, PatientDashboardQueries>();

            // Patient Appointments
            builder.Services.AddScoped<IPatientAppointmentsQueries, PatientAppointmentsQueries>();

            // Patient Booking
            builder.Services.AddScoped<IPatientBookingQueries, PatientBookingQueries>();

            // ✅ Patient Imaging & Radiology
            builder.Services.AddScoped<IPatientImagingQueries, PatientImagingQueries>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
