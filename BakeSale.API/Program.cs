using System;
using BakeSale.Infrastructure;
using BakeSale.Infrastructure.Repositories;
using BakeSale.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using BakeSale.Infrastructure.Data;
using BakeSale.API.Hubs;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using BakeSale.API.Helpers;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.SignalR;


var builder = WebApplication.CreateBuilder(args);

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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<ISessionWrapper, SessionWrapper>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(EFRepository<>));

builder.Services.AddScoped<IDataContext>(provider => provider.GetRequiredService<AppDbContext>());


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder.WithOrigins("https://localhost:62170")
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

if (app.Environment.IsDevelopment())
{
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
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Config", "Images")),
    RequestPath = "/images",
    ServeUnknownFileTypes = true,
    DefaultContentType = "image/png"
});

Console.WriteLine($"Path is {Path.Combine(Directory.GetCurrentDirectory(), "Config", "Images")}");

app.UseSession();

app.UseRouting();

app.UseCors("AllowOrigin");

app.UseAuthorization();

app.MapControllers();

app.MapHub<ProductHub>("/productHub");
app.MapHub<CartHub>("/cartHub");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();

    await dbContext.Database.MigrateAsync();
    var databaseSeeder = new DatabaseSeeder(logger);
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
