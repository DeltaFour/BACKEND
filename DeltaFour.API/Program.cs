using DeltaFour.CrossCutting.Ioc;
using DeltaFour.CrossCutting.Middleware;
using DotNetEnv;
using Python.Runtime;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

String pythonHome = Environment.GetEnvironmentVariable("PYTHON_HOME")!;

// Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome);
// Environment.SetEnvironmentVariable("PYTHONPATH", $"{pythonHome}\\Lib;{pythonHome}\\DLLs");

Runtime.PythonDLL = @"C:\\Users\\Arthur\\AppData\\Local\\Programs\\Python\\Python310\\python310.dll";
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddConfigJwt(builder.Configuration);
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
app.Run();
