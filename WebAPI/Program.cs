using Business.Abstract;
using Business.Concrete;
using Business.Profiles;
using Business.Services;
using Core.Logging;
using DataAccess.Context;
using Entities.Concrete;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.BackgroundJobs;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = false;

            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        var jwt = builder.Configuration.GetSection("Jwt");
        var jwtKey = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing in appsettings.json");

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        builder.Services.AddControllers();



        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<ILoggerService, FileLogger>();

        builder.Services.AddScoped<IInventoryService,InventoryService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IAgingService, AgingService>();
        builder.Services.AddScoped<IProductionService, ProductionService>();
        builder.Services.AddScoped<IWorkerService, WorkerService>();

        builder.Services.AddHostedService<BarnSimulationWorker>();
        builder.Services.AddHostedService<WorkerCollectionJob>();

        var app = builder.Build();

        Console.WriteLine($"ENV: {app.Environment.EnvironmentName}");
        Console.WriteLine("Swagger endpoint: /swagger");


        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BarnManagement API v1");
            c.RoutePrefix = "swagger";
        });

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("/", () => "API is running. Go to /swagger");

        app.Run();
    }
}

//using Business.Abstract;
//using Business.Services;
//using DataAccess.Context;
//using Entities.Concrete;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Reflection;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// -------------------- DB --------------------
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// -------------------- Identity --------------------
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();

//// -------------------- JWT --------------------
//var jwtSettings = builder.Configuration.GetSection("Jwt");
//var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = jwtSettings["Issuer"],
//        ValidAudience = jwtSettings["Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(key)
//    };
//});

//builder.Services.AddAuthorization();

//// -------------------- Swagger --------------------
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BarnManagement API", Version = "v1" });

//    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "JWT Authorization header using the Bearer scheme.",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer"
//    });


//});

//// -------------------- Services --------------------
//builder.Services.AddScoped<IAuthService, AuthService>();

//builder.Services.AddControllers();

//var app = builder.Build();

//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseAuthentication();
//app.UseAuthorization();

//try
//{
//    app.MapControllers();
//}
//catch (ReflectionTypeLoadException ex)
//{
//    Console.WriteLine("=== LoaderExceptions ===");
//    foreach (var e in ex.LoaderExceptions)
//        Console.WriteLine(e?.ToString());
//    throw;
//}

//try
//{
//    app.Run();
//}
//catch (ReflectionTypeLoadException ex)
//{
//    Console.WriteLine("ReflectionTypeLoadException:");
//    Console.WriteLine(ex.Message);

//    if (ex.LoaderExceptions != null)
//    {
//        Console.WriteLine("---- LoaderExceptions ----");
//        foreach (var e in ex.LoaderExceptions)
//            Console.WriteLine(e?.ToString());
//    }

//    throw;
//}


