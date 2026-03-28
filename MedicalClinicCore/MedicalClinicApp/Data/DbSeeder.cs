using MedicalClinicApp.Models;

namespace MedicalClinicApp.Data;

public static class DbSeeder
{
    public static void Seed(ClinicDbContext db)
    {
        if (db.Logins.Any()) return; // Already seeded

        // Offices
        var offices = new[]
        {
            new Office { Address = "123 Main St, Suite 100", Phone = "555-0100" },
            new Office { Address = "456 Oak Ave, Building B", Phone = "555-0200" },
            new Office { Address = "789 Pine Rd, Floor 3", Phone = "555-0300" }
        };
        db.Offices.AddRange(offices);
        db.SaveChanges();

        // Insurance
        var insurances = new[]
        {
            new Insurance { InsuranceName = "Blue Cross", Phone = "800-100-0001" },
            new Insurance { InsuranceName = "Aetna", Phone = "800-200-0002" },
            new Insurance { InsuranceName = "United Health", Phone = "800-300-0003" }
        };
        db.Insurances.AddRange(insurances);
        db.SaveChanges();

        // Doctors
        var doctors = new[]
        {
            new Doctor { FName = "John", LName = "Smith", Email = "john.smith@clinic.com", Phone = "555-1001", Specialization = "Cardiology", Salary = 250000, DOB = new DateTime(1975, 3, 15) },
            new Doctor { FName = "Sarah", LName = "Johnson", Email = "sarah.j@clinic.com", Phone = "555-1002", Specialization = "Neurology", Salary = 240000, DOB = new DateTime(1980, 7, 22) },
            new Doctor { FName = "Michael", LName = "Chen", Email = "m.chen@clinic.com", Phone = "555-1003", Specialization = "Dermatology", Salary = 220000, DOB = new DateTime(1978, 11, 5) },
            new Doctor { FName = "Emily", LName = "Davis", Email = "e.davis@clinic.com", Phone = "555-1004", Specialization = "Internal Medicine", Salary = 210000, DOB = new DateTime(1982, 1, 30) }
        };
        db.Doctors.AddRange(doctors);
        db.SaveChanges();

        // Nurses
        var nurses = new[]
        {
            new Nurse { FName = "Lisa", LName = "Brown", Email = "l.brown@clinic.com", Phone = "555-2001", OfficeID = offices[0].OfficeID, Pay = 75000, DOB = new DateTime(1990, 5, 12) },
            new Nurse { FName = "James", LName = "Wilson", Email = "j.wilson@clinic.com", Phone = "555-2002", OfficeID = offices[1].OfficeID, Pay = 72000, DOB = new DateTime(1988, 9, 8) }
        };
        db.Nurses.AddRange(nurses);
        db.SaveChanges();

        // Staff
        var staff = new[]
        {
            new Staff { FName = "Maria", LName = "Garcia", Email = "m.garcia@clinic.com", Phone = "555-3001", SRole = "Receptionist", OfficeID = offices[0].OfficeID, Salary = 45000, DOB = new DateTime(1992, 4, 18) }
        };
        db.Staff.AddRange(staff);
        db.SaveChanges();

        // Patients
        var patients = new[]
        {
            new Patient { FName = "Robert", LName = "Taylor", Email = "r.taylor@email.com", Phone = "555-4001", Gender = "Male", Address = "100 Elm St", DOB = new DateTime(1985, 6, 20), DoctorID = doctors[0].DoctorID, InsuranceID = insurances[0].InsuranceID },
            new Patient { FName = "Jennifer", LName = "Martinez", Email = "j.martinez@email.com", Phone = "555-4002", Gender = "Female", Address = "200 Maple Ave", DOB = new DateTime(1990, 12, 3), DoctorID = doctors[1].DoctorID, InsuranceID = insurances[1].InsuranceID },
            new Patient { FName = "David", LName = "Anderson", Email = "d.anderson@email.com", Phone = "555-4003", Gender = "Male", Address = "300 Cedar Ln", DOB = new DateTime(1978, 8, 15), DoctorID = doctors[0].DoctorID, InsuranceID = insurances[2].InsuranceID },
            new Patient { FName = "Amanda", LName = "Thomas", Email = "a.thomas@email.com", Phone = "555-4004", Gender = "Female", Address = "400 Birch Dr", DOB = new DateTime(1995, 2, 28), DoctorID = doctors[2].DoctorID },
            new Patient { FName = "William", LName = "Jackson", Email = "w.jackson@email.com", Phone = "555-4005", Gender = "Male", Address = "500 Walnut Ct", DOB = new DateTime(1970, 10, 10), DoctorID = doctors[3].DoctorID, InsuranceID = insurances[0].InsuranceID }
        };
        db.Patients.AddRange(patients);
        db.SaveChanges();

        // Appointments
        var today = DateTime.Today;
        var appointments = new[]
        {
            new Appointment { PatientID = patients[0].PatientID, DoctorID = doctors[0].DoctorID, OfficeID = offices[0].OfficeID, AppointmentDate = today.AddDays(1), AppointmentTime = new TimeSpan(9, 0, 0), Approval = true, PATIENT_CONFIRM = true },
            new Appointment { PatientID = patients[1].PatientID, DoctorID = doctors[1].DoctorID, OfficeID = offices[1].OfficeID, AppointmentDate = today.AddDays(2), AppointmentTime = new TimeSpan(10, 30, 0), Approval = true },
            new Appointment { PatientID = patients[2].PatientID, DoctorID = doctors[0].DoctorID, OfficeID = offices[0].OfficeID, AppointmentDate = today.AddDays(3), AppointmentTime = new TimeSpan(14, 0, 0) },
            new Appointment { PatientID = patients[3].PatientID, DoctorID = doctors[2].DoctorID, OfficeID = offices[2].OfficeID, AppointmentDate = today.AddDays(1), AppointmentTime = new TimeSpan(11, 0, 0), Approval = true, PATIENT_CONFIRM = true },
            new Appointment { PatientID = patients[4].PatientID, DoctorID = doctors[3].DoctorID, OfficeID = offices[1].OfficeID, AppointmentDate = today, AppointmentTime = new TimeSpan(15, 0, 0), Approval = true, NurseID = nurses[0].NID }
        };
        db.Appointments.AddRange(appointments);
        db.SaveChanges();

        // Prescriptions
        var prescriptions = new[]
        {
            new Prescription { PatientID = patients[0].PatientID, DoctorID = doctors[0].DoctorID, Medication = "Lisinopril", Dosage = "10mg", Frequency = "Once daily", Quantity = 30, StartDate = today, EndDate = today.AddDays(30) },
            new Prescription { PatientID = patients[1].PatientID, DoctorID = doctors[1].DoctorID, Medication = "Gabapentin", Dosage = "300mg", Frequency = "Twice daily", Quantity = 60, StartDate = today, EndDate = today.AddDays(30) },
            new Prescription { PatientID = patients[2].PatientID, DoctorID = doctors[0].DoctorID, Medication = "Metoprolol", Dosage = "25mg", Frequency = "Once daily", Quantity = 30, StartDate = today.AddDays(-15), EndDate = today.AddDays(15) }
        };
        db.Prescriptions.AddRange(prescriptions);
        db.SaveChanges();

        // Invoices
        var invoices = new[]
        {
            new Invoice { PatientID = patients[0].PatientID, Total = 350, Claim = 280, PaidAmount = 70, DueDate = today.AddDays(30) },
            new Invoice { PatientID = patients[1].PatientID, Total = 500, Claim = 400, PaidAmount = 0, DueDate = today.AddDays(15) },
            new Invoice { PatientID = patients[2].PatientID, Total = 275, Claim = 200, PaidAmount = 75, DueDate = today.AddDays(-5) }
        };
        db.Invoices.AddRange(invoices);
        db.SaveChanges();

        // Logins
        var logins = new[]
        {
            new Login { Username = "admin", Password = "admin123", AdminID = 1 },
            new Login { Username = "drsmith", Password = "doc123", DoctorID = doctors[0].DoctorID },
            new Login { Username = "drjohnson", Password = "doc123", DoctorID = doctors[1].DoctorID },
            new Login { Username = "drchen", Password = "doc123", DoctorID = doctors[2].DoctorID },
            new Login { Username = "drdavis", Password = "doc123", DoctorID = doctors[3].DoctorID },
            new Login { Username = "nursebrown", Password = "nurse123", NurseID = nurses[0].NID },
            new Login { Username = "nursewilson", Password = "nurse123", NurseID = nurses[1].NID },
            new Login { Username = "rtaylor", Password = "patient123", PatientID = patients[0].PatientID },
            new Login { Username = "jmartinez", Password = "patient123", PatientID = patients[1].PatientID },
            new Login { Username = "danderson", Password = "patient123", PatientID = patients[2].PatientID },
            new Login { Username = "athomas", Password = "patient123", PatientID = patients[3].PatientID },
            new Login { Username = "wjackson", Password = "patient123", PatientID = patients[4].PatientID }
        };
        db.Logins.AddRange(logins);
        db.SaveChanges();

        // Schedules
        var schedules = new[]
        {
            new Schedule { DoctorID = doctors[0].DoctorID, OfficeID = offices[0].OfficeID, Day = "Monday", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
            new Schedule { DoctorID = doctors[0].DoctorID, OfficeID = offices[0].OfficeID, Day = "Wednesday", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(17, 0, 0) },
            new Schedule { DoctorID = doctors[0].DoctorID, OfficeID = offices[0].OfficeID, Day = "Friday", StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(12, 0, 0) },
            new Schedule { DoctorID = doctors[1].DoctorID, OfficeID = offices[1].OfficeID, Day = "Tuesday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0) },
            new Schedule { DoctorID = doctors[1].DoctorID, OfficeID = offices[1].OfficeID, Day = "Thursday", StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(18, 0, 0) }
        };
        db.Schedules.AddRange(schedules);
        db.SaveChanges();
    }
}
