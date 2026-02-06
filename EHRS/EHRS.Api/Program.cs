using EHRS.Core.Interfaces;
using EHRS.Core.Abstractions.Queries;          // ✅ اتضاف
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Services;
using EHRS.Infrastructure.Queries;             // ✅ اتضاف
using Microsoft.EntityFrameworkCore;

namespace EHRS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // DbContext (Database First)
            builder.Services.AddDbContext<EHRSContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("EHRS")));

            // Services (existing)
            builder.Services.AddScoped<IDoctorService, DoctorService>();

            // 🔥 NEW: Appointments Queries (GET /appointments)
            builder.Services.AddScoped<IAppointmentQueries, AppointmentQueries>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
