using CedMT_Test_SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:5175") // Permitir solicitudes desde este origen
               .AllowAnyHeader() // Permitir cualquier cabecera
               .AllowAnyMethod() // Permitir cualquier método (GET, POST, etc.)
               .AllowCredentials(); // Permitir credenciales (necesario para SignalR)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS
app.UseCors("AllowFrontend");

app.MapHub<GridHub>("grid-hub");

app.UseAuthorization();

app.MapControllers();

app.Run();