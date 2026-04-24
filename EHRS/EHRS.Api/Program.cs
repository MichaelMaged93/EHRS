using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;

using EHRS.Api.Localization;
using EHRS.Api.Services;

using EHRS.Core.Abstractions.Queries;
using EHRS.Core.Interfaces;
using EHRS.Core.Settings;

using EHRS.Infrastructure.Persistence;
using EHRS.Infrastructure.Queries;
using EHRS.Infrastructure.Queries.Patients;
using EHRS.Infrastructure.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
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

            // ================= Controllers =================
            builder.Services.AddControllers();

            // ================= Localization =================
            builder.Services.AddLocalization(o => o.ResourcesPath = "Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new AcceptLanguageHeaderRequestCultureProvider()
                };
            });

            // ================= Swagger =================
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EHRS API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
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

            // ================= DbContext =================
            var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Missing connection string");

            builder.Services.AddDbContext<EHRSContext>(options =>
                options.UseSqlServer(connStr));

            // ================= Core Services =================
            builder.Services.AddScoped<IEncryptionService, EncryptionService>();
            builder.Services.AddScoped<IAppLocalizer, AppLocalizer>();
            builder.Services.AddScoped<IEmailService, EmailService>();

            // ================= JWT =================
            builder.Services.AddSingleton<JwtTokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            var jwtKey = builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt Key missing");

            var issuer = builder.Configuration["Jwt:Issuer"] ?? "EHRS";
            var audience = builder.Configuration["Jwt:Audience"] ?? "EHRS.Client";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            // ================= Rate Limiting =================
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("LoginPolicy", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        ip,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 5
                        });
                });

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many requests"
                    }, token);
                };
            });

            // ================= Services =================
            builder.Services.AddScoped<IDoctorService, DoctorService>();
            builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            builder.Services.AddScoped<IDoctorProfileService, DoctorProfileService>();

            // ================= Queries =================
            builder.Services.AddScoped<IAppointmentQueries, AppointmentQueries>();
            builder.Services.AddScoped<IMedicalRecordQueries, MedicalRecordQueries>();
            builder.Services.AddScoped<IDashboardQueries, DashboardQueries>();
            builder.Services.AddScoped<IDoctorProfileQueries, DoctorProfileQueries>();

            builder.Services.AddScoped<IPatientProfileQueries, PatientProfileQueries>();
            builder.Services.AddScoped<IPatientDashboardQueries, PatientDashboardQueries>();
            builder.Services.AddScoped<IPatientAppointmentsQueries, PatientAppointmentsQueries>();
            builder.Services.AddScoped<IPatientBookingQueries, PatientBookingQueries>();
            builder.Services.AddScoped<IPatientPrescriptionsQueries, PatientPrescriptionsQueries>();
            builder.Services.AddScoped<IPatientImagingQueries, PatientImagingQueries>();
            builder.Services.AddScoped<IPatientMedicalHistoryQueries, PatientMedicalHistoryQueries>();

            builder.Services.AddScoped<IDoctorPatientQueries, DoctorPatientQueries>();
            builder.Services.AddScoped<IDoctorSurgeryQueries, DoctorSurgeryQueries>();

            builder.Services.AddScoped<IPatientAuthQueries, PatientAuthQueries>();
            builder.Services.AddScoped<IDoctorAuthQueries, DoctorAuthQueries>();

            // ================= Build App =================
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // ================= STATIC FILES (FINAL FIX) =================
            app.UseStaticFiles(); // wwwroot فقط (ده الصح)

            // ================= Localization =================
            var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            // ================= Middleware =================
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            app.MapControllers();

            app.Run();
        }
    }
}