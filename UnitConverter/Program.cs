using Services;
using UnitConversionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// 1. Register Swagger Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register your core business logic service
builder.Services.AddSingleton<IConversionService, ConversionService>();

var app = builder.Build();

// 2. Enable Swagger UI in Development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Sets the Swagger UI to load at the application root (http://localhost:5000)
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
        options.RoutePrefix = string.Empty;
    });
}

app.MapControllers();
app.Run();