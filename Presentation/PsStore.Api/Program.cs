using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PsStore.Application;
using PsStore.Application.Exceptions;
using PsStore.Application.Features.Category.Rules;
using PsStore.Application.Features.Dlc.Rules;
using PsStore.Application.Features.Game.Rules;
using PsStore.Application.Interfaces.Services;
using PsStore.Infrastructure;
using PsStore.Infrastructure.Services;
using PsStore.Mapper;
using PsStore.Mapper.AutoMapper;
using PsStore.Mapper.AutoMapper.Profiles;
using PsStore.Persistance;
using PsStore.Persistance.Context;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//  Configure Serilog Logging
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console()
          .WriteTo.MSSqlServer(
              connectionString: context.Configuration.GetConnectionString("DefaultConnection"),
              sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
              {
                  TableName = "Logs",
                  AutoCreateSqlTable = true
              });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var env = builder.Environment;

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSingleton<PsStore.Application.Interfaces.AutoMapper.IMapper, Mapper>();

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

builder.Services.AddPersistance(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddCustomMapper();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<GameRules>();
builder.Services.AddScoped<DlcRules>();
builder.Services.AddScoped<ErrorLoggingService>();
builder.Services.AddScoped<IErrorLoggingService, ErrorLoggingService>();
builder.Services.AddScoped<CategoryRules>();




//  Configure Swagger with Authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Youtube API", Version = "v1", Description = "Youtube API Swagger client" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Type `Bearer` and after space, paste token"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

//  Add Serilog Middleware
app.UseSerilogRequestLogging(); // Logs HTTP requests

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

app.Use(async (context, next) =>
{
    try
    {
        logger.LogInformation("Incoming Request: {Method} {Path} at {Time}",
            context.Request.Method, context.Request.Path, DateTime.UtcNow);

        await next();

        logger.LogInformation("Response Sent: {StatusCode} at {Time}",
            context.Response.StatusCode, DateTime.UtcNow);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Unhandled exception occurred while processing request.");
        throw;
    }
});



// Use Middleware for Global Exception Handling
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
