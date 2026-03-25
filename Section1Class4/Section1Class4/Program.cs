var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? apiKey = builder.Configuration["AISettings:ApiKey"];
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("No se encontró la clave de API. Verifica la configuración.");
}
else
{
    Console.WriteLine("Clave de API cargada correctamente.");
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "La API está funcionando correctamente");

app.Run();