using OpenAI.Chat;
using Azure;
using Azure.AI.OpenAI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? apiKey = builder.Configuration["AISettings:AzureApiKey"];
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

app.MapPost("/api/ai/ask", async (string prompt) =>
{
    var endpoint = new Uri("https://proyectosoporteia.openai.azure.com/");
    var deploymentName = "gpt-4.1-nano";

    AzureOpenAIClient azureClient = new(
    endpoint,
    new AzureKeyCredential(apiKey));

    ChatClient chatClient = azureClient.GetChatClient(deploymentName);

    var requestOptions = new ChatCompletionOptions()
    {
        MaxOutputTokenCount = 50,
        Temperature = 0.7f,
        TopP = 0.9f,
        FrequencyPenalty = 0.2f,
        PresencePenalty = 0.2f,
    };

    List<ChatMessage> messages = new List<ChatMessage>()
    {
        new SystemChatMessage("You are a helpful assistant."),
        new UserChatMessage(prompt),
    };

    var response = chatClient.CompleteChat(messages, requestOptions);

    var result = response.Value.Content[0].Text;

    return Results.Ok(result);
});

app.Run();

