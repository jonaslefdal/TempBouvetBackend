using Microsoft.EntityFrameworkCore;
using BouvetBackend.DataAccess;
using BouvetBackend.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder.WithOrigins(
            "http://localhost:8889",            
            "http://localhost:3000",
            "http://localhost:3000"            
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()); 
});

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IApiRepository, EfApiRepository>();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5279);
    });


var app = builder.Build();

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();
