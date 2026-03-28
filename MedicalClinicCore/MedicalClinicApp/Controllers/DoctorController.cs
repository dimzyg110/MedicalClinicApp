using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Data;
using MedicalClinicApp.Models;
using MedicalClinicApp.ViewModels;

namespace MedicalClinicApp.Controllers;

public class DoctorController : Controller
{
    private readonly ClinicDbContext _db;
    public DoctorController(ClinicDbContext db) => _db = db;

    private int? GetDoctorID() => HttpContext.Session.GetInt32("UserID");
    private bool IsDoctor() => HttpContext.Session.GetString("Role") == "Doctor";

    public async Task<IActionResult> Dashboard()
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        var doctorID = GetDoctorID()!.Value;

        var doctor = await _db.Doctors.FindAsync(doctorID);
        if (doctor == null) return NotFound();

        var today = DateTime.Today;
        var dayName = today.DayOfWeek.ToString();

        var schedule = await _db.Schedules
            .Include(s => s.Office)
            .FirstOrDefaultAsync(s => s.DoctorID == doctorID && s.Day == dayName);

        var upcoming = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Office)
            .Where(a => a.DoctorID == doctorID && a.AppointmentDate >= today && !a.Archive && (a.Referral || a.Referral == false))
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentTime)
            .ToListAsync();

        var vm = new DoctorDashboardViewModel
        {
            Doctor = doctor,
            UpcomingAppointments = upcoming,
            OfficeLocation = schedule?.Office?.Address ?? "No office assigned today"
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveAppointment(int id)
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        var appt = await _db.Appointments.FindAsync(id);
        if (appt != null)
        {
            appt.Approval = true;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> CancelAppointment(int id, string? reason)
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        var appt = await _db.Appointments.FindAsync(id);
        if (appt != null)
        {
            appt.Archive = true;
            appt.CancellationReason = reason ?? "Cancelled by doctor";
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> Patients()
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        var doctorID = GetDoctorID()!.Value;
        var patients = await _db.Patients
            .Where(p => p.DoctorID == doctorID && !p.Archive)
            .ToListAsync();
        return View(patients);
    }

    public async Task<IActionResult> Prescriptions()
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        var doctorID = GetDoctorID()!.Value;
        var prescriptions = await _db.Prescriptions
            .Include(p => p.Patient)
            .Where(p => p.DoctorID == doctorID)
            .OrderByDescending(p => p.StartDate)
            .ToListAsync();
        return View(prescriptions);
    }

    [HttpGet]
    public async Task<IActionResult> CreatePrescription(int? patientId)
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        var doctorID = GetDoctorID()!.Value;
        ViewBag.Patients = await _db.Patients.Where(p => p.DoctorID == doctorID && !p.Archive).ToListAsync();
        ViewBag.SelectedPatientId = patientId;
        return View(new Prescription { DoctorID = doctorID, PatientID = patientId });
    }

    [HttpPost]
    public async Task<IActionResult> CreatePrescription(Prescription rx)
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        rx.DoctorID = GetDoctorID()!.Value;
        _db.Prescriptions.Add(rx);
        await _db.SaveChangesAsync();
        return RedirectToAction("Prescriptions");
    }

    public async Task<IActionResult> CreateReport(int appointmentId)
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        var appt = await _db.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
        if (appt == null) return NotFound();
        ViewBag.Appointment = appt;
        ViewBag.Evaluations = await _db.Evaluations.ToListAsync();
        return View(new VisitDetails { AppointmentID = appointmentId, PatientID = appt.PatientID, DoctorID = GetDoctorID()!.Value });
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport(VisitDetails report)
    {
        if (!IsDoctor()) return RedirectToAction("DoctorLogin", "Account");
        report.DoctorID = GetDoctorID()!.Value;
        _db.VisitDetails.Add(report);
        await _db.SaveChangesAsync();

        var appt = await _db.Appointments.FindAsync(report.AppointmentID);
        if (appt != null)
        {
            appt.ReportID = report.ReportID;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction("Dashboard");
    }
}
