using DeltaFour.CrossCutting.Ioc;
using DeltaFour.CrossCutting.Middleware;
using DotNetEnv;
using Python.Runtime;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

string dll = Environment.GetEnvironmentVariable("PYTHONNET_PYDLL")!;

Runtime.PythonDLL = dll;
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

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

AppDomain.CurrentDomain.ProcessExit += (s, e) => PythonEngine.Shutdown();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.ApplyMigrationsAndSeedAsync();

app.Run();
