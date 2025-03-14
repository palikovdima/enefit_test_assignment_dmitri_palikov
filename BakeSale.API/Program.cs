using System;
using BakeSale.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.SignalR;
using API.Configurations.Swagger;
using API.Configurations.Settings;
using Domain.Service;
using Domain.Models;
using API.Configurations.Session;
using API.Hubs;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Service.Environment;
using Infrastructure.Repositories.Product;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var environment = builder.Environment.EnvironmentName;

var appSettings = new AppSettings();
configuration.GetSection("AppSettings").Bind(appSettings);

EnvironmentSettings? envSettings = environment == "Development" ? appSettings.Development : appSettings.Production;

builder.Services.Configure<EnvironmentSettings>(options =>
{
    options.FrontendUrl = envSettings!.FrontendUrl;
    options.BackendUrl = envSettings!.BackendUrl;
    options.ImagePath = envSettings!.ImagePath;
    options.CSVPath = envSettings!.CSVPath;
    options.StaticFilesPath = envSettings!.StaticFilesPath;
    options.CurrenciesPath = envSettings!.CurrenciesPath;
    options.CurrencyImagesPath = envSettings!.CurrencyImagesPath;
});

Console.WriteLine($"Frontend url : {envSettings!.FrontendUrl!}, Backend url: {envSettings!.BackendUrl!}");

builder.Services.AddSingleton(envSettings!);
builder.Services.AddSingleton<EnvironmentSettingsService>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/bakesale_log.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<ISessionWrapper, SessionWrapper>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

builder.Services.AddScoped<ProductRepository>();

builder.Services.AddScoped<IDataContext>(provider => provider.GetRequiredService<AppDbContext>());


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder
            .WithOrigins(envSettings!.BackendUrl!, envSettings!.FrontendUrl!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});


builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

builder.Services.Configure<SessionOptions>(options =>
{
    options.Cookie.Name = ".MyApp.Session";
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }

    options.RoutePrefix = "swagger";
});


app.UseHttpsRedirection();

app.UseStaticFiles();

string basePath = AppContext.BaseDirectory;
string imagesPath = Path.GetFullPath(envSettings!.ImagePath!);

if (!Directory.Exists(imagesPath))
{ 
    Console.WriteLine($"Path does not exist: {imagesPath}");
}

Console.WriteLine($"Path is {imagesPath}");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = envSettings!.StaticFilesPath!,
    ServeUnknownFileTypes = true,
    DefaultContentType = "image/png"
});

Console.WriteLine($"Path is {imagesPath}");

app.UseSession();

app.UseRouting();

app.UseCors("AllowOrigin");

app.UseAuthorization();

app.MapControllers();

app.MapHub<ProductHub>("/api/v1.0/productHub");
app.MapHub<CartHub>("/api/v1.0/cartHub");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();

    var envSettingsService = scope.ServiceProvider.GetRequiredService<EnvironmentSettingsService>();
    var env = app.Services.GetRequiredService<IWebHostEnvironment>();

    Console.WriteLine($"Content Root: {env.ContentRootPath}");
    Console.WriteLine($"Environment: {env.EnvironmentName}");

    await dbContext.Database.MigrateAsync();
    var databaseSeeder = new DatabaseSeeder(logger, new EnvironmentSettingsService(envSettings));
    await databaseSeeder.SeedAsync(dbContext);
}

app.Use(async (context, next) =>
{
    var testSession = context.Session.GetString("TestKey");

    if (string.IsNullOrEmpty(testSession))
    {
        Console.WriteLine("Session is empty! Setting test key.");
        context.Session.SetString("TestKey", "Session Working");
    }
    else
    {
        Console.WriteLine($"Session is persisting! Value: {testSession}");
    }

    await next();
});

app.Run();
