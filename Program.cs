using BibliotecaAPI;
using BibliotecaAPI.Datos;
using BibliotecaAPI.Entidades;
using BibliotecaAPI.Servicios;
using BibliotecaAPI.Swagger;
using BibliotecaAPI.Utilidades;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//add services to the container

/*builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(15);
}); */

builder.Services.AddStackExchangeRedisOutputCache(opciones =>
{
    opciones.Configuration = builder.Configuration.GetConnectionString("redis");
});

builder.Services.AddDataProtection();

var origenesPermitidos = builder.Configuration.GetSection("origenesPermitidos").Get<string[]>()!;

builder.Services.AddCors(opciones=>
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
    .WithExposedHeaders("cantidad-total-registros")
    );
}
    );

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers(opciones=>
{
    opciones.Filters.Add<FiltroTiempoEjecucion>();
}).AddNewtonsoftJson();

builder.Services.AddDbContext<AplicationDBContext>(options =>
    options.UseSqlServer("name=DefaultConnection"));


builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<AplicationDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<Usuario>>();
builder.Services.AddScoped<SignInManager<Usuario>>();
builder.Services.AddTransient<IServiciosUsuarios, ServiciosUsuarios>();
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();
builder.Services.AddScoped<MiFiltroDeAccion>();
builder.Services.AddScoped<FiltroValidacionLibro>();
builder.Services.AddScoped<BibliotecaAPI.Servicios.V1.IServiciosAutores, BibliotecaAPI.Servicios.V1.ServiciosAutores>();



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

    opciones.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    opciones.OperationFilter<FiltroAutorizacion>();



   /* opciones.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type= ReferenceType.SecurityScheme,
                Id= "Bearer"
            }
        },
            new string[]{}

        }
    }); */

});


var app = builder.Build();

// add middleware 

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        var excepcion = exceptionHandlerFeature?.Error!;

        var error = new Error()
        {
            MensajeDeError = excepcion.Message,
            StrackTrace = excepcion.StackTrace,
            Fecha = DateTime.UtcNow
        };

        var dbContext = context.RequestServices.GetRequiredService<AplicationDBContext>();
        dbContext.Add(error);
        await dbContext.SaveChangesAsync();
        await Results.InternalServerError( new
        {
            Tipo = "error",
            mensaje = "Ha ocurrido un error inesperado",
            estatus = 500
        }).ExecuteAsync (context);
    }));

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors();


app.MapControllers();


// Ver error 
/* app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR GLOBAL:");
        Console.WriteLine(ex.Message);
        throw;
    }
}); */ 



app.Run();
