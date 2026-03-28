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

// Database connection - try multiple approaches
string connectionString;

// Log all MySQL-related env vars for debugging
Console.WriteLine("=== MySQL Environment Variables ===");
foreach (var key in new[] { "DATABASE_URL", "MYSQL_URL", "MYSQL_PUBLIC_URL", "MYSQLHOST", "MYSQLUSER", "MYSQLPASSWORD", "MYSQLDATABASE", "MYSQLPORT" })
{
    var val = Environment.GetEnvironmentVariable(key);
    if (key.Contains("PASSWORD") && !string.IsNullOrEmpty(val))
        Console.WriteLine($"  {key} = ***SET*** (length={val.Length})");
    else
        Console.WriteLine($"  {key} = {val ?? "(not set)"}");
}
Console.WriteLine("===================================");

// Strategy 1: Try DATABASE_URL or MYSQL_URL first (full URL)
var urlString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? Environment.GetEnvironmentVariable("MYSQL_URL")
    ?? Environment.GetEnvironmentVariable("MYSQL_PUBLIC_URL");

if (!string.IsNullOrEmpty(urlString) && urlString.StartsWith("mysql://"))
{
    var uri = new Uri(urlString);
    var userInfo = uri.UserInfo.Split(':');
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var user = Uri.UnescapeDataString(userInfo[0]);
    var database = uri.AbsolutePath.TrimStart('/');
    var host = uri.Host;
    var port2 = uri.Port > 0 ? uri.Port : 3306;
    connectionString = $"Server={host};Port={port2};Database={database};User={user};Password={password};SslMode=Preferred;AllowPublicKeyRetrieval=true;ConnectionTimeout=30;";
    Console.WriteLine($"Using MySQL URL: Server={host}, Port={port2}, Database={database}, User={user}");
}
// Strategy 2: Use individual env vars
else
{
    var mysqlHost = Environment.GetEnvironmentVariable("MYSQLHOST");
    var mysqlUser = Environment.GetEnvironmentVariable("MYSQLUSER");
    var mysqlPassword = Environment.GetEnvironmentVariable("MYSQLPASSWORD");
    var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQLDATABASE");
    var mysqlPort = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";

    if (!string.IsNullOrEmpty(mysqlHost) && !string.IsNullOrEmpty(mysqlUser))
    {
        connectionString = $"Server={mysqlHost};Port={mysqlPort};Database={mysqlDatabase};User={mysqlUser};Password={mysqlPassword};SslMode=Preferred;AllowPublicKeyRetrieval=true;ConnectionTimeout=30;";
        Console.WriteLine($"Using individual MySQL vars: Server={mysqlHost}, Port={mysqlPort}, Database={mysqlDatabase}, User={mysqlUser}");
    }
    else
    {
        connectionString = "Server=localhost;Port=3306;Database=medical_clinic;User=root;Password=root;SslMode=Preferred;AllowPublicKeyRetrieval=true;ConnectionTimeout=30;";
        Console.WriteLine("WARNING: No MySQL env vars found, using localhost fallback");
    }
}

var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));
builder.Services.AddDbContext<ClinicDbContext>(options =>
    options.UseMySql(connectionString, serverVersion, mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

var app = builder.Build();

// Migrate and seed database with retries
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ClinicDbContext>();
    for (int attempt = 1; attempt <= 3; attempt++)
    {
        try
        {
            Console.WriteLine($"Database setup attempt {attempt}/3...");
            Console.WriteLine("Testing connection...");
            await db.Database.CanConnectAsync();
            Console.WriteLine("Connection successful!");
            Console.WriteLine("Running EnsureCreated...");
            db.Database.EnsureCreated();
            Console.WriteLine("Database created/verified. Seeding...");
            DbSeeder.Seed(db);
            Console.WriteLine("Seeding complete.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Attempt {attempt} failed: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"Inner: {ex.InnerException.Message}");
            if (attempt < 3)
            {
                Console.WriteLine("Waiting 5 seconds before retry...");
                await Task.Delay(5000);
            }
            else
            {
                Console.WriteLine("All attempts failed. App will start but DB may not be ready.");
            }
        }
    }
}

// Always show detailed errors for now (temporary for debugging)
app.UseDeveloperExceptionPage();

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
