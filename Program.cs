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


app.MapControllers();

app.Run();
