using Microsoft.EntityFrameworkCore;
using MedicalClinicApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Database - Build connection string from Railway's individual MYSQL env vars
string connectionString;

var mysqlHost = Environment.GetEnvironmentVariable("MYSQLHOST");
var mysqlUser = Environment.GetEnvironmentVariable("MYSQLUSER");
var mysqlPassword = Environment.GetEnvironmentVariable("MYSQLPASSWORD");
var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQLDATABASE");
var mysqlPort = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";

if (!string.IsNullOrEmpty(mysqlHost) && !string.IsNullOrEmpty(mysqlUser))
{
    // Use individual Railway MySQL variables (most reliable)
    connectionString = $"Server={mysqlHost};Port={mysqlPort};Database={mysqlDatabase};User={mysqlUser};Password={mysqlPassword};SslMode=Preferred;AllowPublicKeyRetrieval=true;";
    Console.WriteLine($"Using Railway MySQL vars: Server={mysqlHost}, Port={mysqlPort}, Database={mysqlDatabase}, User={mysqlUser}");
}
else
{
    // Fallback: try MYSQL_URL or DATABASE_URL
    var urlString = Environment.GetEnvironmentVariable("MYSQL_URL")
        ?? Environment.GetEnvironmentVariable("DATABASE_URL");

    if (!string.IsNullOrEmpty(urlString) && urlString.StartsWith("mysql://"))
    {
        var uri = new Uri(urlString);
        var userInfo = uri.UserInfo.Split(':');
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var user = Uri.UnescapeDataString(userInfo[0]);
        var database = uri.AbsolutePath.TrimStart('/');
        connectionString = $"Server={uri.Host};Port={uri.Port};Database={database};User={user};Password={password};SslMode=Preferred;AllowPublicKeyRetrieval=true;";
        Console.WriteLine($"Using MySQL URL: Server={uri.Host}, Port={uri.Port}, Database={database}");
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=medical_clinic;User=root;Password=root;SslMode=Preferred;AllowPublicKeyRetrieval=true;";
        Console.WriteLine("Using default/local connection string");
    }
}

var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

var app = builder.Build();

// Migrate and seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
    try
    {
        Console.WriteLine("Running database migration...");
        db.Database.Migrate();
        Console.WriteLine("Migration complete. Seeding...");
        DbSeeder.Seed(db);
        Console.WriteLine("Seeding complete.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration error: {ex.Message}");
        try
        {
            Console.WriteLine("Trying EnsureCreated fallback...");
            db.Database.EnsureCreated();
            DbSeeder.Seed(db);
            Console.WriteLine("EnsureCreated + Seed complete.");
        }
        catch (Exception ex2)
        {
            Console.WriteLine($"Database create fallback error: {ex2.Message}");
        }
    }
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Handle forwarded headers from Railway proxy
app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("X-Forwarded-Proto"))
    {
        context.Request.Scheme = context.Request.Headers["X-Forwarded-Proto"].ToString();
    }
    await next();
});

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

Console.WriteLine($"Starting on port {port}...");
app.Run();
