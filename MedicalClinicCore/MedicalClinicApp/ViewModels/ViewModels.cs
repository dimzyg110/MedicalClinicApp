using MedicalClinicApp.Models;

namespace MedicalClinicApp.ViewModels;

public class LoginViewModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string? ErrorMessage { get; set; }
}

public class DoctorDashboardViewModel
{
    public Doctor Doctor { get; set; } = null!;
    public List<Appointment> UpcomingAppointments { get; set; } = new();
    public List<Appointment> ReferralAppointments { get; set; } = new();
    public string? OfficeLocation { get; set; }
}

public class NurseDashboardViewModel
{
    public Nurse Nurse { get; set; } = null!;
    public List<Appointment> TodayAppointments { get; set; } = new();
}

public class PatientDashboardViewModel
{
    public Patient Patient { get; set; } = null!;
    public List<Appointment> UpcomingAppointments { get; set; } = new();
    public List<Appointment> PastAppointments { get; set; } = new();
    public List<Prescription> Prescriptions { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
}

public class AdminDashboardViewModel
{
    public int TotalDoctors { get; set; }
    public int TotalNurses { get; set; }
    public int TotalPatients { get; set; }
    public int TotalAppointments { get; set; }
    public int TotalOffices { get; set; }
    public List<Doctor> Doctors { get; set; } = new();
    public List<Nurse> Nurses { get; set; } = new();
    public List<Staff> StaffMembers { get; set; } = new();
    public List<Office> Offices { get; set; } = new();
}

public class NewAppointmentViewModel
{
    public int? PatientID { get; set; }
    public int DoctorID { get; set; }
    public int OfficeID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public List<Doctor> AvailableDoctors { get; set; } = new();
    public List<Office> AvailableOffices { get; set; } = new();
}

public class NewPersonnelViewModel
{
    public string Type { get; set; } = "Doctor"; // Doctor, Nurse, Staff
    public string FName { get; set; } = "";
    public string? MName { get; set; }
    public string LName { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Specialization { get; set; }
    public int? Salary { get; set; }
    public int? OfficeID { get; set; }
    public DateTime? DOB { get; set; }
    public string? Role { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public List<Office> Offices { get; set; } = new();
}

public class ReportViewModel
{
    public VisitDetails Report { get; set; } = null!;
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
    public Appointment? Appointment { get; set; }
    public List<Prescription> Prescriptions { get; set; } = new();
    public List<MedicalTest> Tests { get; set; } = new();
}
