using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalClinicApp.Models;

[Table("doctor")]
public class Doctor
{
    [Key]
    public int DoctorID { get; set; }
    [Column("fname")] public string FName { get; set; } = "";
    [Column("mname")] public string? MName { get; set; }
    [Column("lname")] public string LName { get; set; } = "";
    public string? Specialization { get; set; }
    public DateTime? DOB { get; set; }
    [Column("Work_email")] public string? Email { get; set; }
    [Column("Phone_num")] public string? Phone { get; set; }
    [Column("archive")] public bool Archive { get; set; }
    public int? Salary { get; set; }

    [NotMapped] public string FullName => $"Dr. {FName} {LName}";
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

[Table("nurse")]
public class Nurse
{
    [Key]
    public int NID { get; set; }
    public string FName { get; set; } = "";
    public string? MName { get; set; }
    public string LName { get; set; } = "";
    [Column("Work_email")] public string? Email { get; set; }
    [Column("pay")] public decimal? Pay { get; set; }
    [Column("officeID")] public int? OfficeID { get; set; }
    [Column("Phone_num")] public string? Phone { get; set; }
    public DateTime? DOB { get; set; }
    [Column("archive")] public bool Archive { get; set; }

    [NotMapped] public string FullName => $"{FName} {LName}";
    public Office? Office { get; set; }
}

[Table("patients")]
public class Patient
{
    [Key]
    public int PatientID { get; set; }
    [Column("fname")] public string FName { get; set; } = "";
    [Column("mname")] public string? MName { get; set; }
    [Column("lname")] public string LName { get; set; } = "";
    public DateTime? DOB { get; set; }
    [Column("email")] public string? Email { get; set; }
    [Column("phone")] public string? Phone { get; set; }
    [Column("address")] public string? Address { get; set; }
    [Column("gender")] public string? Gender { get; set; }
    [Column("doctorID")] public int? DoctorID { get; set; }
    [Column("insuranceID")] public int? InsuranceID { get; set; }
    [Column("archive")] public bool Archive { get; set; }

    [NotMapped] public string FullName => $"{FName} {LName}";
    public Doctor? Doctor { get; set; }
    public Insurance? Insurance { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

[Table("appointment")]
public class Appointment
{
    [Key]
    public int AppointmentID { get; set; }
    [Column("doctorID")] public int DoctorID { get; set; }
    [Column("patientID")] public int? PatientID { get; set; }
    public bool Approval { get; set; }
    [Column("reportID")] public int? ReportID { get; set; }
    [Column("officeID")] public int OfficeID { get; set; }
    public bool Referral { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public TimeSpan? AppointmentTime { get; set; }
    public bool? PATIENT_CONFIRM { get; set; }
    [Column("archive")] public bool Archive { get; set; }
    [Column("cancellation_reason")] public string? CancellationReason { get; set; }
    [Column("nurseID")] public int? NurseID { get; set; }

    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public Office? Office { get; set; }
    public VisitDetails? VisitDetails { get; set; }
}

[Table("office")]
public class Office
{
    [Key]
    [Column("officeID")] public int OfficeID { get; set; }
    [Column("officeAddress")] public string? Address { get; set; }
    [Column("officePhone")] public string? Phone { get; set; }
    [Column("archive")] public bool Archive { get; set; }
}

[Table("login")]
public class Login
{
    [Key]
    [Column("username")] public string Username { get; set; } = "";
    [Column("passwrd")] public string Password { get; set; } = "";
    [Column("doctorID")] public int? DoctorID { get; set; }
    [Column("patientID")] public int? PatientID { get; set; }
    [Column("nurseID")] public int? NurseID { get; set; }
    [Column("staffID")] public int? StaffID { get; set; }
    [Column("adminID")] public int? AdminID { get; set; }
}

[Table("staff")]
public class Staff
{
    [Key]
    public int StaffID { get; set; }
    public string? FName { get; set; }
    public string? LName { get; set; }
    public string? SRole { get; set; }
    public string? Pay { get; set; }
    [Column("officeID")] public int? OfficeID { get; set; }
    public DateTime? DOB { get; set; }
    [Column("Work_email")] public string? Email { get; set; }
    [Column("Phone_num")] public string? Phone { get; set; }
    [Column("archive")] public bool Archive { get; set; }
    public int? Salary { get; set; }
    public Office? Office { get; set; }
}

[Table("insurance")]
public class Insurance
{
    [Key]
    public int InsuranceID { get; set; }
    [Column("group_no")] public string? GroupNo { get; set; }
    [Column("Insurance_Name")] public string? InsuranceName { get; set; }
    [Column("patientID")] public int? PatientID { get; set; }
    [Column("phone")] public string? Phone { get; set; }
}

[Table("prescriptions")]
public class Prescription
{
    [Key]
    [Column("prescriptionID")] public int PrescriptionID { get; set; }
    [Column("medication")] public string? Medication { get; set; }
    [Column("dosage")] public string? Dosage { get; set; }
    [Column("frequency")] public string? Frequency { get; set; }
    [Column("start_date")] public DateTime? StartDate { get; set; }
    [Column("end_date")] public DateTime? EndDate { get; set; }
    [Column("patientID")] public int? PatientID { get; set; }
    [Column("doctorID")] public int? DoctorID { get; set; }
    [Column("reportID")] public int? ReportID { get; set; }
    [Column("quantity")] public int? Quantity { get; set; }
    [Column("notes")] public string? Notes { get; set; }

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}

[Table("visit_details")]
public class VisitDetails
{
    [Key]
    [Column("reportID")] public int ReportID { get; set; }
    [Column("diagnosis")] public string? Diagnosis { get; set; }
    [Column("treatment")] public string? Treatment { get; set; }
    [Column("notes")] public string? Notes { get; set; }
    [Column("height")] public decimal? Height { get; set; }
    [Column("weight")] public decimal? Weight { get; set; }
    [Column("blood_pressure")] public string? BloodPressure { get; set; }
    [Column("temperature")] public decimal? Temperature { get; set; }
    [Column("heart_rate")] public int? HeartRate { get; set; }
    [Column("patientID")] public int? PatientID { get; set; }
    [Column("doctorID")] public int? DoctorID { get; set; }
    [Column("nurseID")] public int? NurseID { get; set; }
    [Column("appointmentID")] public int? AppointmentID { get; set; }

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}

[Table("invoice")]
public class Invoice
{
    [Key]
    [Column("invoiceID")] public int InvoiceID { get; set; }
    [Column("total")] public decimal? Total { get; set; }
    [Column("claim")] public decimal? Claim { get; set; }
    [Column("paid_amount")] public decimal? PaidAmount { get; set; }
    [Column("patientID")] public int? PatientID { get; set; }
    [Column("reportID")] public int? ReportID { get; set; }
    [Column("due_date")] public DateTime? DueDate { get; set; }

    public Patient? Patient { get; set; }
}

[Table("evaluation")]
public class Evaluation
{
    [Key]
    [Column("code")] public int Code { get; set; }
    [Column("test")] public string? Test { get; set; }
}

[Table("test")]
public class MedicalTest
{
    [Key]
    [Column("testID")] public int TestID { get; set; }
    [Column("patientID")] public int? PatientID { get; set; }
    [Column("doctorID")] public int? DoctorID { get; set; }
    [Column("code")] public int? Code { get; set; }
    [Column("results")] public string? Results { get; set; }
    [Column("reportID")] public int? ReportID { get; set; }

    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public Evaluation? Evaluation { get; set; }
}

[Table("schedule")]
public class Schedule
{
    [Key]
    [Column("scheduleID")] public int ScheduleID { get; set; }
    [Column("doctorID")] public int? DoctorID { get; set; }
    [Column("officeID")] public int? OfficeID { get; set; }
    [Column("day")] public string? Day { get; set; }
    [Column("start_time")] public TimeSpan? StartTime { get; set; }
    [Column("end_time")] public TimeSpan? EndTime { get; set; }

    public Doctor? Doctor { get; set; }
    public Office? Office { get; set; }
}

[Table("emergency_contact_patient")]
public class EmergencyContactPatient
{
    [Key]
    [Column("Name")] public string Name { get; set; } = "";
    [Column("Relationship")] public string? Relationship { get; set; }
    [Column("Phone")] public string? Phone { get; set; }
    [Column("Email")] public string? Email { get; set; }
    [Column("patientID")] public int PatientID { get; set; }
}

[Table("emergency_contact_employee")]
public class EmergencyContactEmployee
{
    [Key]
    [Column("Name")] public string Name { get; set; } = "";
    [Column("Relationship")] public string? Relationship { get; set; }
    [Column("Phone")] public string? Phone { get; set; }
    [Column("Email")] public string? Email { get; set; }
    [Column("employeeID")] public int EmployeeID { get; set; }
    [Column("employeeType")] public string? EmployeeType { get; set; }
}
