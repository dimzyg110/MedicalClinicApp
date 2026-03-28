using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Data;
using MedicalClinicApp.Models;

namespace MedicalClinicApp.Controllers;

public class HomeController : Controller
{
    private readonly ClinicDbContext _db;

    public HomeController(ClinicDbContext db)
    {
        _db = db;
    }

    public IActionResult Index() => View();

    public async Task<IActionResult> Doctors()
    {
        var doctors = await _db.Doctors.Where(d => !d.Archive).ToListAsync();
        return View(doctors);
    }

    public IActionResult About() => View();
    public IActionResult Contact() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
