using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Data;
using MedicalClinicApp.ViewModels;

namespace MedicalClinicApp.Controllers;

public class AccountController : Controller
{
    private readonly ClinicDbContext _db;
    public AccountController(ClinicDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var login = await _db.Logins.FirstOrDefaultAsync(l =>
            l.Username == model.Username && l.Password == model.Password);

        if (login == null)
        {
            model.ErrorMessage = "Invalid username or password.";
            return View(model);
        }

        HttpContext.Session.SetString("Username", login.Username);

        if (login.AdminID != null)
        {
            HttpContext.Session.SetString("Role", "Admin");
            HttpContext.Session.SetInt32("UserID", login.AdminID.Value);
            return RedirectToAction("Index", "Admin");
        }
        if (login.DoctorID != null)
        {
            HttpContext.Session.SetString("Role", "Doctor");
            HttpContext.Session.SetInt32("UserID", login.DoctorID.Value);
            return RedirectToAction("Dashboard", "Doctor");
        }
        if (login.NurseID != null)
        {
            HttpContext.Session.SetString("Role", "Nurse");
            HttpContext.Session.SetInt32("UserID", login.NurseID.Value);
            return RedirectToAction("Dashboard", "Nurse");
        }
        if (login.PatientID != null)
        {
            HttpContext.Session.SetString("Role", "Patient");
            HttpContext.Session.SetInt32("UserID", login.PatientID.Value);
            return RedirectToAction("Dashboard", "Patient");
        }

        model.ErrorMessage = "Account has no assigned role.";
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult PatientLogin() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> PatientLogin(LoginViewModel model)
    {
        var login = await _db.Logins.FirstOrDefaultAsync(l =>
            l.Username == model.Username && l.Password == model.Password && l.PatientID != null);
        if (login == null)
        {
            model.ErrorMessage = "Invalid patient credentials.";
            return View(model);
        }
        HttpContext.Session.SetString("Username", login.Username);
        HttpContext.Session.SetString("Role", "Patient");
        HttpContext.Session.SetInt32("UserID", login.PatientID!.Value);
        return RedirectToAction("Dashboard", "Patient");
    }

    [HttpGet]
    public IActionResult DoctorLogin() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> DoctorLogin(LoginViewModel model)
    {
        var login = await _db.Logins.FirstOrDefaultAsync(l =>
            l.Username == model.Username && l.Password == model.Password && l.DoctorID != null);
        if (login == null)
        {
            model.ErrorMessage = "Invalid doctor credentials.";
            return View(model);
        }
        HttpContext.Session.SetString("Username", login.Username);
        HttpContext.Session.SetString("Role", "Doctor");
        HttpContext.Session.SetInt32("UserID", login.DoctorID!.Value);
        return RedirectToAction("Dashboard", "Doctor");
    }

    [HttpGet]
    public IActionResult NurseLogin() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> NurseLogin(LoginViewModel model)
    {
        var login = await _db.Logins.FirstOrDefaultAsync(l =>
            l.Username == model.Username && l.Password == model.Password && l.NurseID != null);
        if (login == null)
        {
            model.ErrorMessage = "Invalid nurse credentials.";
            return View(model);
        }
        HttpContext.Session.SetString("Username", login.Username);
        HttpContext.Session.SetString("Role", "Nurse");
        HttpContext.Session.SetInt32("UserID", login.NurseID!.Value);
        return RedirectToAction("Dashboard", "Nurse");
    }

    [HttpGet]
    public IActionResult AdminLogin() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> AdminLogin(LoginViewModel model)
    {
        var login = await _db.Logins.FirstOrDefaultAsync(l =>
            l.Username == model.Username && l.Password == model.Password && l.AdminID != null);
        if (login == null)
        {
            model.ErrorMessage = "Invalid admin credentials.";
            return View(model);
        }
        HttpContext.Session.SetString("Username", login.Username);
        HttpContext.Session.SetString("Role", "Admin");
        HttpContext.Session.SetInt32("UserID", login.AdminID!.Value);
        return RedirectToAction("Index", "Admin");
    }
}
