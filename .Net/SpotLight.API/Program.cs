using SpotLight.API.Models;

using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------
// 1. CONFIGURACIÓN DE BASE DE DATOS
// ---------------------------------------------------------
builder.Services.Configure<SpotLightDatabaseSettings>(
    builder.Configuration.GetSection("SpotLightDatabase"));

// ---------------------------------------------------------
// *** REGISTRO DE SERVICIOS (Conexión con Mongo) ***
// Aquí conectamos los "Gerentes" de datos
// ---------------------------------------------------------
builder.Services.AddSingleton<SpotLight.API.Services.ProjectsService>();

// 👇 ESTA ES LA NUEVA LÍNEA PARA LAS EVALUACIONES 👇
builder.Services.AddSingleton<SpotLight.API.Services.EvaluationsService>();


// ---------------------------------------------------------
// 2. HABILITAR CORS
// Permite que la Web y el Celular se conecten sin bloqueo
// ---------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => 
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
// Configuración de documentación (Swagger/OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

var app = builder.Build();

// ---------------------------------------------------------
// 3. PIPELINE DE LA APLICACIÓN
// ---------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ¡IMPORTANTE! Usar CORS antes de la autorización
app.UseCors("AllowAll");

app.UseDefaultFiles(); 
app.UseStaticFiles();  

app.UseAuthorization();

app.MapControllers();

app.Run();