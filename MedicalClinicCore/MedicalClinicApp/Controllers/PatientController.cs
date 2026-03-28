using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Data;
using MedicalClinicApp.Models;
using MedicalClinicApp.ViewModels;

namespace MedicalClinicApp.Controllers;

public class PatientController : Controller
{
    private readonly ClinicDbContext _db;
    public PatientController(ClinicDbContext db) => _db = db;

    private int? GetPatientID() => HttpContext.Session.GetInt32("UserID");
    private bool IsPatient() => HttpContext.Session.GetString("Role") == "Patient";

    public async Task<IActionResult> Dashboard()
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var patientID = GetPatientID()!.Value;

        var patient = await _db.Patients.Include(p => p.Doctor).Include(p => p.Insurance).FirstOrDefaultAsync(p => p.PatientID == patientID);
        if (patient == null) return NotFound();

        var today = DateTime.Today;

        var upcoming = await _db.Appointments
            .Include(a => a.Doctor).Include(a => a.Office)
            .Where(a => a.PatientID == patientID && a.AppointmentDate >= today && !a.Archive)
            .OrderBy(a => a.AppointmentDate).ThenBy(a => a.AppointmentTime)
            .ToListAsync();

        var past = await _db.Appointments
            .Include(a => a.Doctor).Include(a => a.Office)
            .Where(a => a.PatientID == patientID && (a.AppointmentDate < today || a.Archive))
            .OrderByDescending(a => a.AppointmentDate)
            .Take(10).ToListAsync();

        var prescriptions = await _db.Prescriptions
            .Include(p => p.Doctor)
            .Where(p => p.PatientID == patientID)
            .OrderByDescending(p => p.StartDate).ToListAsync();

        var invoices = await _db.Invoices
            .Where(i => i.PatientID == patientID)
            .OrderByDescending(i => i.DueDate).ToListAsync();

        return View(new PatientDashboardViewModel
        {
            Patient = patient,
            UpcomingAppointments = upcoming,
            PastAppointments = past,
            Prescriptions = prescriptions,
            Invoices = invoices
        });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmAppointment(int id)
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var appt = await _db.Appointments.FindAsync(id);
        if (appt != null && appt.PatientID == GetPatientID())
        {
            appt.PATIENT_CONFIRM = true;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> CancelAppointment(int id)
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var appt = await _db.Appointments.FindAsync(id);
        if (appt != null && appt.PatientID == GetPatientID())
        {
            appt.Archive = true;
            appt.CancellationReason = "Cancelled by patient";
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var patient = await _db.Patients.FindAsync(GetPatientID()!.Value);
        return View(patient);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(Patient model)
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var patient = await _db.Patients.FindAsync(GetPatientID()!.Value);
        if (patient == null) return NotFound();

        patient.FName = model.FName;
        patient.LName = model.LName;
        patient.Email = model.Email;
        patient.Phone = model.Phone;
        patient.Address = model.Address;
        patient.Gender = model.Gender;
        await _db.SaveChangesAsync();
        return RedirectToAction("Dashboard");
    }

    [HttpGet]
    public async Task<IActionResult> NewAppointment()
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var doctors = await _db.Doctors.Where(d => !d.Archive).ToListAsync();
        var offices = await _db.Offices.Where(o => !o.Archive).ToListAsync();
        return View(new NewAppointmentViewModel
        {
            PatientID = GetPatientID(),
            AvailableDoctors = doctors,
            AvailableOffices = offices
        });
    }

    [HttpPost]
    public async Task<IActionResult> NewAppointment(NewAppointmentViewModel model)
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var appt = new Appointment
        {
            PatientID = GetPatientID(),
            DoctorID = model.DoctorID,
            OfficeID = model.OfficeID,
            AppointmentDate = model.AppointmentDate,
            AppointmentTime = model.AppointmentTime,
            Approval = false,
            PATIENT_CONFIRM = false,
            Archive = false
        };
        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Appointment scheduled successfully!";
        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> Billing()
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var invoices = await _db.Invoices
            .Where(i => i.PatientID == GetPatientID())
            .OrderByDescending(i => i.DueDate).ToListAsync();
        return View(invoices);
    }

    public async Task<IActionResult> Reports()
    {
        if (!IsPatient()) return RedirectToAction("PatientLogin", "Account");
        var reports = await _db.VisitDetails
            .Include(v => v.Doctor)
            .Where(v => v.PatientID == GetPatientID())
            .ToListAsync();
        return View(reports);
    }
}
