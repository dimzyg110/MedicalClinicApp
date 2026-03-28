using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Data;
using MedicalClinicApp.Models;
using MedicalClinicApp.ViewModels;

namespace MedicalClinicApp.Controllers;

public class NurseController : Controller
{
    private readonly ClinicDbContext _db;
    public NurseController(ClinicDbContext db) => _db = db;

    private int? GetNurseID() => HttpContext.Session.GetInt32("UserID");
    private bool IsNurse() => HttpContext.Session.GetString("Role") == "Nurse";

    public async Task<IActionResult> Dashboard()
    {
        if (!IsNurse()) return RedirectToAction("NurseLogin", "Account");
        var nurseID = GetNurseID()!.Value;

        var nurse = await _db.Nurses.Include(n => n.Office).FirstOrDefaultAsync(n => n.NID == nurseID);
        if (nurse == null) return NotFound();

        var today = DateTime.Today;
        var appointments = await _db.Appointments
            .Include(a => a.Patient).Include(a => a.Doctor).Include(a => a.Office)
            .Where(a => a.NurseID == nurseID && a.AppointmentDate == today && !a.Archive)
            .OrderBy(a => a.AppointmentTime)
            .ToListAsync();

        // If no nurse-specific appointments, show all for the office
        if (!appointments.Any() && nurse.OfficeID.HasValue)
        {
            appointments = await _db.Appointments
                .Include(a => a.Patient).Include(a => a.Doctor).Include(a => a.Office)
                .Where(a => a.OfficeID == nurse.OfficeID && a.AppointmentDate == today && !a.Archive)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        return View(new NurseDashboardViewModel
        {
            Nurse = nurse,
            TodayAppointments = appointments
        });
    }

    [HttpGet]
    public async Task<IActionResult> CreateReport(int appointmentId)
    {
        if (!IsNurse()) return RedirectToAction("NurseLogin", "Account");
        var appt = await _db.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
        if (appt == null) return NotFound();
        ViewBag.Appointment = appt;
        return View(new VisitDetails { AppointmentID = appointmentId, PatientID = appt.PatientID, NurseID = GetNurseID() });
    }

    [HttpPost]
    public async Task<IActionResult> CreateReport(VisitDetails report)
    {
        if (!IsNurse()) return RedirectToAction("NurseLogin", "Account");
        report.NurseID = GetNurseID();
        _db.VisitDetails.Add(report);
        await _db.SaveChangesAsync();

        var appt = await _db.Appointments.FindAsync(report.AppointmentID);
        if (appt != null)
        {
            appt.ReportID = report.ReportID;
            await _db.SaveChangesAsync();
        }
        TempData["Success"] = "Report created successfully!";
        return RedirectToAction("Dashboard");
    }

    public async Task<IActionResult> PatientReports()
    {
        if (!IsNurse()) return RedirectToAction("NurseLogin", "Account");
        var reports = await _db.VisitDetails
            .Include(v => v.Patient).Include(v => v.Doctor)
            .Where(v => v.NurseID == GetNurseID())
            .ToListAsync();
        return View(reports);
    }

    public async Task<IActionResult> Prescriptions()
    {
        if (!IsNurse()) return RedirectToAction("NurseLogin", "Account");
        var nurseID = GetNurseID()!.Value;
        var nurse = await _db.Nurses.FindAsync(nurseID);

        // Show prescriptions for patients at the nurse's office
        var prescriptions = await _db.Prescriptions
            .Include(p => p.Patient).Include(p => p.Doctor)
            .OrderByDescending(p => p.StartDate)
            .Take(50)
            .ToListAsync();
        return View(prescriptions);
    }
}
