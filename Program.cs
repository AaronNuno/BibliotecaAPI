using BibliotecaAPI;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//add services to the container

builder.Services.AddDataProtection();

var origenesPermitidos = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;

builder.Services.AddCors(opciones=>
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
    .WithExposedHeaders("mi-cabecera")
    );
}
    );

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<AplicationDBContext>(options =>
    options.UseSqlServer("name=DefaultConnection"));


builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<AplicationDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();



builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]!)),
        ClockSkew = TimeSpan.Zero


    };

});

builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politca => politca.RequireClaim("esadmin"));
});

builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Biblioteca API",
        Description = "Este es un API para trabajar con datos de autores y libros",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Email = "aaron_a_7@hotmail.com",
            Name = "Aaron Nuno",
            Url = new Uri("https://www.linkedin.com/in/aaron-hern%C3%A1ndez-637378107/")
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://mit-license.org/")
        }
    });
});



var app = builder.Build();

// add middleware 
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();


app.MapControllers();

app.Run();
