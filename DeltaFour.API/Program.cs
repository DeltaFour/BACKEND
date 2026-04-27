using DeltaFour.API.Filters;
using DeltaFour.CrossCutting.Ioc;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json.Serialization;

Env.Load();
var builder = WebApplication.CreateBuilder(args);
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(new Serilog.Formatting.Compact.CompactJsonFormatter(), "logs/log-.json", rollingInterval: Serilog.RollingInterval.Day)
    .WriteTo.MySQL(connectionString ?? string.Empty, tableName: "Logs", restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<GlobalExceptionFilter>();
        options.Filters.Add<RequestLoggingFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DeltaFour.Application.Validators.UserRequestValidator>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddConfigJwt(builder.Configuration);
builder.Services.AddPolicies(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.UseAllOfToExtendReferenceSchemas();

    options.MapType<TimeOnly>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "string",
        Format = "time",
        Example = new Microsoft.OpenApi.Any.OpenApiString("19:00:00")
    });

    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "DeltaFour.API",
        Version = "0.0.1",
        Description = "API da plataforma DeltaFour.",
    });
});

var allowedHost = Environment.GetEnvironmentVariable("ALLOWED_HOST")!;
var frontendCors = "frontendCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: frontendCors,
        policy =>
        {
            policy
                .WithOrigins(allowedHost)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors(frontendCors);
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseMiddleware<DeltaFour.API.Middleware.SubscriptionValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();

await app.ApplyMigrationsAndSeedAsync();

app.Run();

public partial class Program { }
