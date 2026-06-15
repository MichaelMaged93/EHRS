using System;
using System.Collections.Generic;
using EHRS.Core.Interfaces;
using EHRS.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EHRS.Infrastructure.Persistence;

public partial class EHRSContext : DbContext
{
    private readonly IEncryptionService? _encryptionService;

    public EHRSContext()
    {
    }

    public EHRSContext(DbContextOptions<EHRSContext> options)
        : base(options)
    {
    }

    public EHRSContext(DbContextOptions<EHRSContext> options, IEncryptionService encryptionService)
        : base(options)
    {
        _encryptionService = encryptionService;
    }

    public virtual DbSet<Appointment> Appointments { get; set; }
    public virtual DbSet<Doctor> Doctors { get; set; }
    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }
    public virtual DbSet<Patient> Patients { get; set; }
    public virtual DbSet<SensorDatum> SensorData { get; set; }
    public virtual DbSet<SurgeryHistory> SurgeryHistories { get; set; }
    public virtual DbSet<UserCredential> UserCredentials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(
            "Server=.;Database=EHR_Wearable_DB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===================== ENCRYPTION =====================
        if (_encryptionService != null)
        {
            var converter = new ValueConverter<string, string>(
                v => _encryptionService.Encrypt(v ?? string.Empty),
                v => _encryptionService.Decrypt(v ?? string.Empty)
            );

            modelBuilder.Entity<Doctor>(entity =>
            {
                // اختياري
                // entity.Property(e => e.Email).HasConversion(converter);
                // entity.Property(e => e.ContactNumber).HasConversion(converter);
            });

            modelBuilder.Entity<MedicalRecord>(entity =>
            {
                entity.Property(e => e.ChiefComplaint).HasConversion(converter);
                entity.Property(e => e.Diagnosis).HasConversion(converter);
                entity.Property(e => e.ClinicalNotes).HasConversion(converter);
                entity.Property(e => e.Treatment).HasConversion(converter);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(e => e.Address).HasConversion(converter);
                // entity.Property(e => e.Ssn).HasConversion(converter);
            });
        }

        // ===================== APPOINTMENT =====================
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId);

            entity.ToTable("Appointment");

            entity.HasIndex(e => new { e.DoctorId, e.AppointmentDateTime })
                .HasDatabaseName("IX_Appointment_DoctorDate");

            entity.HasIndex(e => new { e.PatientId, e.AppointmentDateTime })
                .HasDatabaseName("IX_Appointment_PatientDate");

            entity.HasOne(d => d.Doctor)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId);

            entity.HasOne(d => d.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId);
        });

        // ===================== DOCTOR =====================
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId);
            entity.ToTable("Doctor");

            entity.HasIndex(e => e.Area);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.About).HasMaxLength(1000);
        });

        // ===================== MEDICAL RECORD =====================
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId);
            entity.ToTable("MedicalRecord");

            entity.Property(e => e.ChiefComplaint).HasMaxLength(300);
            entity.Property(e => e.Diagnosis).HasMaxLength(500);
            entity.Property(e => e.Treatment).HasMaxLength(500);
        });

        // ===================== PATIENT =====================
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId);
            entity.ToTable("Patient");

            entity.HasIndex(e => e.Ssn).IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(200);
        });

        // ===================== SENSOR DATA (FIXED) =====================
        modelBuilder.Entity<SensorDatum>(entity =>
        {
            entity.HasKey(e => e.SensorDataId);

            entity.HasIndex(e => new { e.PatientId, e.Timestamp });

            entity.Property(e => e.ActivityLevel).HasMaxLength(50);
            entity.Property(e => e.SpO2).HasColumnType("decimal(5,2)");
            entity.Property(e => e.Temperature).HasColumnType("decimal(5,2)");

            // ✅ FIXED: removed old PressureHeart completely
            entity.Property(e => e.SystolicPressure).HasColumnName("SystolicPressure");
            entity.Property(e => e.DiastolicPressure).HasColumnName("DiastolicPressure");

            entity.HasOne(d => d.Patient)
                .WithMany(p => p.SensorData)
                .HasForeignKey(d => d.PatientId);
        });

        // ===================== SURGERY =====================
        modelBuilder.Entity<SurgeryHistory>(entity =>
        {
            entity.HasKey(e => e.SurgeryId);
            entity.ToTable("SurgeryHistory");

            entity.HasOne(d => d.Patient)
                .WithMany(p => p.SurgeryHistories)
                .HasForeignKey(d => d.PatientId);

            entity.HasOne(d => d.Doctor)
                .WithMany(p => p.SurgeryHistories)
                .HasForeignKey(d => d.DoctorId);
        });

        // ===================== USER CREDENTIAL =====================
        modelBuilder.Entity<UserCredential>(entity =>
        {
            entity.HasKey(e => e.CredentialId);
            entity.ToTable("UserCredential");

            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.ResetToken).HasMaxLength(200);
            entity.Property(e => e.Role).HasMaxLength(20);

            entity.HasOne(d => d.Patient)
                .WithOne(p => p.UserCredential)
                .HasForeignKey<UserCredential>(d => d.PatientId);

            entity.HasOne(d => d.Doctor)
                .WithOne(p => p.UserCredential)
                .HasForeignKey<UserCredential>(d => d.DoctorId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}