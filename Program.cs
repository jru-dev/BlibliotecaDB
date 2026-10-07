using Biblioteca.Web.Repositorios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Repositorios (Dapper). Se inyectan por constructor en los controladores.
builder.Services.AddScoped<ILibroRepositorio, LibroRepositorio>();
builder.Services.AddScoped<ISocioRepositorio, SocioRepositorio>();
builder.Services.AddScoped<IPrestamoRepositorio, PrestamoRepositorio>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Libros}/{action=Index}/{id?}");

app.Run();
