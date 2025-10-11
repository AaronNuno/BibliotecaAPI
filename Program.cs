var builder = WebApplication.CreateBuilder(args);

//add services to the container

builder.Services.AddControllers();


var app = builder.Build();

// add middleware 

app.Run();
