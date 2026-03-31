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

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/api/support/status", () =>
{
    return Results.Ok(new
    {
        message = "Servicio de Soporte IA funcionando correctamente."
    });
});

app.MapGet("/api/ai/test", async () =>
{
    using var httpClient = new HttpClient();

    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

    var requestBody = new
    {
        model = "gpt-4.1-nano",
        input = "Explica en una frase qué es .NET"
    };

    var response = await httpClient.PostAsJsonAsync(
        "https://api.openai.com/v1/responses",
        requestBody
    );

    var result = await response.Content.ReadAsStringAsync();

    return Results.Ok(result);
});

app.Run();

