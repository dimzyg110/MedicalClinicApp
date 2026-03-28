using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Data;
using MedicalClinicApp.Models;
using MedicalClinicApp.ViewModels;

namespace MedicalClinicApp.Controllers;

public class AdminController : Controller
{
    private readonly ClinicDbContext _db;
    public AdminController(ClinicDbContext db) => _db = db;

    private bool IsAdmin() => HttpContext.Session.GetString("Role") == "Admin";

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");

        var vm = new AdminDashboardViewModel
        {
            TotalDoctors = await _db.Doctors.CountAsync(d => !d.Archive),
            TotalNurses = await _db.Nurses.CountAsync(n => !n.Archive),
            TotalPatients = await _db.Patients.CountAsync(p => !p.Archive),
            TotalAppointments = await _db.Appointments.CountAsync(a => !a.Archive),
            TotalOffices = await _db.Offices.CountAsync(o => !o.Archive),
            Doctors = await _db.Doctors.Where(d => !d.Archive).ToListAsync(),
            Nurses = await _db.Nurses.Where(n => !n.Archive).ToListAsync(),
            StaffMembers = await _db.Staff.Where(s => !s.Archive).ToListAsync(),
            Offices = await _db.Offices.Where(o => !o.Archive).ToListAsync()
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> AddPersonnel()
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        var offices = await _db.Offices.Where(o => !o.Archive).ToListAsync();
        return View(new NewPersonnelViewModel { Offices = offices });
    }

    [HttpPost]
    public async Task<IActionResult> AddPersonnel(NewPersonnelViewModel model)
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");

        switch (model.Type)
        {
            case "Doctor":
                var doc = new Doctor
                {
                    FName = model.FName, MName = model.MName, LName = model.LName,
                    Email = model.Email, Phone = model.Phone,
                    Specialization = model.Specialization,
                    Salary = model.Salary, DOB = model.DOB
                };
                _db.Doctors.Add(doc);
                await _db.SaveChangesAsync();
                if (!string.IsNullOrEmpty(model.Username))
                {
                    _db.Logins.Add(new Login { Username = model.Username, Password = model.Password ?? "123", DoctorID = doc.DoctorID });
                    await _db.SaveChangesAsync();
                }
                break;

            case "Nurse":
                var nurse = new Nurse
                {
                    FName = model.FName, MName = model.MName, LName = model.LName,
                    Email = model.Email, Phone = model.Phone,
                    OfficeID = model.OfficeID, DOB = model.DOB,
                    Pay = model.Salary
                };
                _db.Nurses.Add(nurse);
                await _db.SaveChangesAsync();
                if (!string.IsNullOrEmpty(model.Username))
                {
                    _db.Logins.Add(new Login { Username = model.Username, Password = model.Password ?? "123", NurseID = nurse.NID });
                    await _db.SaveChangesAsync();
                }
                break;

            case "Staff":
                var staff = new Staff
                {
                    FName = model.FName, LName = model.LName,
                    Email = model.Email, Phone = model.Phone,
                    SRole = model.Role, OfficeID = model.OfficeID,
                    DOB = model.DOB, Salary = model.Salary
                };
                _db.Staff.Add(staff);
                await _db.SaveChangesAsync();
                if (!string.IsNullOrEmpty(model.Username))
                {
                    _db.Logins.Add(new Login { Username = model.Username, Password = model.Password ?? "123", StaffID = staff.StaffID });
                    await _db.SaveChangesAsync();
                }
                break;
        }

        TempData["Success"] = $"{model.Type} added successfully!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemovePersonnel(string type, int id)
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");

        switch (type)
        {
            case "Doctor":
                var doc = await _db.Doctors.FindAsync(id);
                if (doc != null) { doc.Archive = true; await _db.SaveChangesAsync(); }
                break;
            case "Nurse":
                var nurse = await _db.Nurses.FindAsync(id);
                if (nurse != null) { nurse.Archive = true; await _db.SaveChangesAsync(); }
                break;
            case "Staff":
                var staff = await _db.Staff.FindAsync(id);
                if (staff != null) { staff.Archive = true; await _db.SaveChangesAsync(); }
                break;
        }
        TempData["Success"] = $"{type} removed successfully!";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> AddOffice()
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        return View(new Office());
    }

    [HttpPost]
    public async Task<IActionResult> AddOffice(Office office)
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        _db.Offices.Add(office);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Office added successfully!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveOffice(int id)
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        var office = await _db.Offices.FindAsync(id);
        if (office != null)
        {
            office.Archive = true;
            await _db.SaveChangesAsync();
        }
        TempData["Success"] = "Office removed successfully!";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Appointments()
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        var appointments = await _db.Appointments
            .Include(a => a.Doctor).Include(a => a.Patient).Include(a => a.Office)
            .Where(a => !a.Archive)
            .OrderBy(a => a.AppointmentDate)
            .ToListAsync();
        return View(appointments);
    }

    public async Task<IActionResult> Invoices()
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        var invoices = await _db.Invoices
            .Include(i => i.Patient)
            .OrderByDescending(i => i.DueDate)
            .ToListAsync();
        return View(invoices);
    }

    public async Task<IActionResult> Reports()
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        var reports = await _db.VisitDetails
            .Include(v => v.Doctor).Include(v => v.Patient)
            .ToListAsync();
        return View(reports);
    }

    [HttpGet]
    public async Task<IActionResult> CreateLogin()
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        ViewBag.Doctors = await _db.Doctors.Where(d => !d.Archive).ToListAsync();
        ViewBag.Nurses = await _db.Nurses.Where(n => !n.Archive).ToListAsync();
        ViewBag.Staff = await _db.Staff.Where(s => !s.Archive).ToListAsync();
        return View(new Login());
    }

    [HttpPost]
    public async Task<IActionResult> CreateLogin(Login login)
    {
        if (!IsAdmin()) return RedirectToAction("AdminLogin", "Account");
        _db.Logins.Add(login);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Login created successfully!";
        return RedirectToAction("Index");
    }
}
