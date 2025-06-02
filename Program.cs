using Microsoft.EntityFrameworkCore;
using BouvetBackend.DataAccess;
using BouvetBackend.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using BouvetBackend.Services;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000", 
            "https://jonaslefdal.github.io", 
            "https://jonaslefdal.github.io/", 
            "https://vickynygaard.github.io",
            "https://vickynygaard.github.io/", 
            "https://jonaslefdal.github.io/BouvetApp", 
            "https://jonaslefdal.github.io/BouvetApp/", 
            "https://vickynygaard.github.io/Kortreist", 
            "https://vickynygaard.github.io/Kortreist/",
            "https://a9fj38dkfj3lsd8f7a3lfj2n93xj2lfkd93jf02nd9r3nf83ndk3fj.loca.lt/",
            "https://a9fj38dkfj3lsd8f7a3lfj2n93xj2lfkd93jf02nd9r3nf83ndk3fj.loca.lt") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); 
});

    builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    builder.Services.AddOpenApi();


    builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
           .EnableSensitiveDataLogging(false));

    builder.Services.AddScoped<IApiRepository, EfApiRepository>();
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
    builder.Services.AddScoped<IChallengeRepository, EfChallengeRepository>();
    builder.Services.AddScoped<IUserChallengeProgressRepository, EfUserChallengeProgressRepository>();
    builder.Services.AddScoped<ITransportEntryRepository, EfTransportEntryRepository>();
    builder.Services.AddScoped<ICompanyRepository, EfCompanyRepository>();
    builder.Services.AddScoped<ITeamRepository, EfTeamRepository>();
    builder.Services.AddScoped<IAchievementRepository, EfAchievementRepository>();
    builder.Services.AddScoped<IEndUserAddressRepository, EfEndUserAddressRepository>();
    builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();
    builder.Services.AddHttpClient<IDistanceService, DistanceService>();
    builder.Services.AddScoped<ChallengeProgressService>();

/*
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5279);
    });
*/
    var azureAdB2CConfig = builder.Configuration.GetSection("AzureAdB2C");

    // Microsoft Idetity Web token validering
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));

    /*builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token successfully validated.");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                Console.WriteLine($"Token received: {context.Token}");
                return Task.CompletedTask;
            }
        };
    });*/

    builder.Services.AddAuthorization();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<DataContext>();
            context.Database.Migrate();  
            //context.Database.EnsureDeleted();  
            //context.Database.EnsureCreated();  

        }
        catch (Exception ex)
        {
            Console.WriteLine("Database migration/seeding error: " + ex.Message);
        }
    }

    app.UseRouting();
    app.UseCors("AllowFrontend");
    app.UseAuthentication(); 
    app.UseAuthorization();

    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwaggerUi(options =>
        {
            options.DocumentPath = "openapi/v1.json";
        });
    }


app.Run();
