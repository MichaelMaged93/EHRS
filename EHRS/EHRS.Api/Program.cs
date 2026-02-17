using System.Globalization;
using System.Text;
using EHRS.Api.Localization;
using EHRS.Api.Services;
using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Interfaces;
using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Queries;
using EHRS.Infrastructure.Queries.Patients;
using EHRS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace EHRS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Controllers
            builder.Services.AddControllers();

            // ✅ Localization (ar / en) - Resources folder
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // Priority: Accept-Language header
                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });

            // Swagger + Bearer Auth
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EHRS API", Version = "v1" });

                // ✅ Add JWT Bearer to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // DbContext
            var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connStr))
                throw new InvalidOperationException("Connection string 'DefaultConnection' is missing.");

            builder.Services.AddDbContext<EHRSContext>(options =>
                options.UseSqlServer(connStr));

            // ✅ JWT Token Service (Api)
            builder.Services.AddSingleton<JwtTokenService>();

            // ✅ JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
            var issuer = builder.Configuration["Jwt:Issuer"] ?? "EHRS";
            var audience = builder.Configuration["Jwt:Audience"] ?? "EHRS.Client";

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            // ✅ Helper: App Localizer (Messages.resx / Messages.ar.resx)
            builder.Services.AddScoped<IAppLocalizer, AppLocalizer>();

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

            // Patient Imaging & Radiology
            builder.Services.AddScoped<IPatientImagingQueries, PatientImagingQueries>();

            // ✅ Auth Queries (Patient/Doctor)
            builder.Services.AddScoped<IPatientAuthQueries, PatientAuthQueries>();
            builder.Services.AddScoped<IDoctorAuthQueries, DoctorAuthQueries>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // ✅ Apply Localization (must be before Authentication/Authorization)
            var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            // ✅ لازم Authentication قبل Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
