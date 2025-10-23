using BibliotecaAPI;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Utilidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//add services to the container
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<AplicationDBContext>(options =>
    options.UseSqlServer("name=DefaultConnection"));

var app = builder.Build();

// add middleware 


// Log
/* app.Use(async (contexto, next) =>
{
    // Viene petición
    var logger = contexto.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Peitición: {contexto.Request.Method} {contexto.Request.Path}");

    await next.Invoke();
    
    // Se va la petición

    logger.LogInformation($"Respuesta: {contexto.Response.StatusCode}");
});

app.Use(async (contexto, next) =>
{
    if(contexto.Request.Path == "/bloqueado")
    {
        contexto.Response.StatusCode = 403;
        await contexto.Response.WriteAsync("Acceso denegado");
    }
    else
    {
        await next.Invoke();
    }

}); */


app.MapControllers();

app.Run();
