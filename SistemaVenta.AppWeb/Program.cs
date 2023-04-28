using Microsoft.EntityFrameworkCore;
using SistemaVentas.AppWeb.Utilidades.AutoMapper;

using SistemaVentas.IOC;

using SistemaVentas.AppWeb.Utilidades.Extensiones;
using DinkToPdf;
using DinkToPdf.Contracts;

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("CadenaSQL");

//Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
      .AddCookie(opt =>
      {
          opt.LoginPath = "/Acceso/Login";
          opt.ExpireTimeSpan = TimeSpan.FromMinutes(20);
      });

builder.Services.AddDbContext<DbContext>(options =>
    options.UseSqlServer(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//Todas las dependecias que esten en IOC seran cargadas hacia la appWeb
builder.Services.InyectarDependencia(builder.Configuration);

//Inyectado la config dentro de automapper, para usarlo en todo el proy
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

var context = new CustomAssemblyLoadContext();
context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Utilidades/libreriaPDF/libwkhtmltox.dll"));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");
//pattern: "{controller=Home}/{action=index}/{id?}");

app.Run();
