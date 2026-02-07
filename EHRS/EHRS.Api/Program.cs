using EHRS.Core.Interfaces;
using EHRS.Core.Abstractions.Queries;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Services;
using EHRS.Infrastructure.Queries;
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

            // ✅ Doctor Profile Service (NEW)
            builder.Services.AddScoped<IDoctorProfileService, DoctorProfileService>();

            // Queries
            builder.Services.AddScoped<IAppointmentQueries, AppointmentQueries>();
            builder.Services.AddScoped<IDashboardQueries, DashboardQueries>();
            builder.Services.AddScoped<IMedicalRecordQueries, MedicalRecordQueries>();
            builder.Services.AddScoped<IDoctorProfileQueries, DoctorProfileQueries>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ✅ مهم لعرض الصور وملفات PDF اللي هتتخزن في wwwroot/uploads
            app.UseStaticFiles();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
