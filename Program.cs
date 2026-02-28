using Microsoft.EntityFrameworkCore;
using UFUKDER_BAGIS.Models;
using UFUKDER_BAGIS.Services.Concrete;
using UFUKDER_BAGIS.Services.Interfaces;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Senin yazdýðýn loglar için
    .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal) // Framework loglarýný kapat
    .MinimumLevel.Override("System", LogEventLevel.Fatal)    // System loglarýný kapat
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day, shared: true)
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
