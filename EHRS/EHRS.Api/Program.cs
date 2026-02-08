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

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<EHRSContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("EHRS")));

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

            // Patient Dashboard
            builder.Services.AddScoped<IPatientDashboardQueries, PatientDashboardQueries>();

            // ✅ Patient Appointments (Upcoming + Cancel)
            builder.Services.AddScoped<IPatientAppointmentsQueries, PatientAppointmentsQueries>();

            // ✅ Patient Booking (Areas/Specialties/Doctors + Create)
            builder.Services.AddScoped<IPatientBookingQueries, PatientBookingQueries>();

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
