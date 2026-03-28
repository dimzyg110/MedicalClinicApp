using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Models;

namespace MedicalClinicApp.Data;

public class ClinicDbContext : DbContext
{
    public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Nurse> Nurses => Set<Nurse>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Office> Offices => Set<Office>();
    public DbSet<Login> Logins => Set<Login>();
    public DbSet<Staff> Staff => Set<Staff>();
    public DbSet<Insurance> Insurances => Set<Insurance>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<VisitDetails> VisitDetails => Set<VisitDetails>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Evaluation> Evaluations => Set<Evaluation>();
    public DbSet<MedicalTest> Tests => Set<MedicalTest>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<EmergencyContactPatient> EmergencyContactPatients => Set<EmergencyContactPatient>();
    public DbSet<EmergencyContactEmployee> EmergencyContactEmployees => Set<EmergencyContactEmployee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Login>().HasKey(l => l.Username);
        modelBuilder.Entity<EmergencyContactPatient>().HasKey(e => new { e.Name, e.PatientID });
        modelBuilder.Entity<EmergencyContactEmployee>().HasKey(e => new { e.Name, e.EmployeeID });

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorID);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientID);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Office)
            .WithMany()
            .HasForeignKey(a => a.OfficeID);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.VisitDetails)
            .WithMany()
            .HasForeignKey(a => a.ReportID);
    }
}
