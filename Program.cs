using Microsoft.EntityFrameworkCore;
using BouvetBackend.DataAccess;
using BouvetBackend.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using BouvetBackend.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000", "https://jonaslefdal.github.io", "https://jonaslefdal.github.io/", "https://jonaslefdal.github.io/BouvetApp", "https://jonaslefdal.github.io/BouvetApp/, https://vickynygaard.github.io/Kortreist, https://vickynygaard.github.io/Kortreist/") 
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()); 
});

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IApiRepository, EfApiRepository>();
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
    builder.Services.AddScoped<IChallengeRepository, EfChallengeRepository>();
    builder.Services.AddScoped<IUserChallengeAttemptRepository, EfUserChallengeAttemptRepository>();
    builder.Services.AddScoped<ITransportEntryRepository, EfTransportEntryRepository>();
    builder.Services.AddScoped<ICompanyRepository, EfCompanyRepository>();
    builder.Services.AddScoped<ITeamRepository, EfTeamRepository>();
    builder.Services.AddScoped<IAchievementRepository, EfAchievementRepository>();
    builder.Services.AddHttpClient<IGeocodingService, GeocodingService>();
    builder.Services.AddHttpClient<IDistanceService, DistanceService>();

/*
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5279);
    });
*/
    var azureAdB2CConfig = builder.Configuration.GetSection("AzureAdB2C");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Use configuration values
            var instance = azureAdB2CConfig["Instance"];
            var signUpSignInPolicyId = azureAdB2CConfig["SignUpSignInPolicyId"];
            var domain = azureAdB2CConfig["Domain"];
            var clientId = azureAdB2CConfig["ClientId"];
            var validIssuer = azureAdB2CConfig["ValidIssuer"];

            options.Authority = $"{instance}{domain}/{signUpSignInPolicyId}";
            options.Audience = clientId;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = validIssuer,
                ValidateAudience = true,
                ValidAudience = clientId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };

           /* options.Events = new JwtBearerEvents
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
                }
            };*/
        });

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
