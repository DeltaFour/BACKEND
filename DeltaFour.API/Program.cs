using DeltaFour.CrossCutting.Ioc;
using DeltaFour.CrossCutting.Middleware;
using DotNetEnv;
using Python.Runtime;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

String dll = Environment.GetEnvironmentVariable("PYTHONNET_PYDLL")!;

Runtime.PythonDLL = dll;
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
         
     });
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddConfigJwt(builder.Configuration);
builder.Services.AddPolicies(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

AppDomain.CurrentDomain.ProcessExit += (s, e) => PythonEngine.Shutdown();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.ApplyMigrationsAndSeedAsync();

app.Run();
