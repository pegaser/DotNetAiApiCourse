using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;

public class AiService
{
    private readonly ChatClient _client;
    public AiService(IConfiguration configuration)
    {
        var endpoint = configuration["AISettings:Endpoint"] ?? "";
        var apiKey = configuration["AISettings:AzureApiKey"] ?? "";
        var deployment = configuration["AISettings:DeploymentName"] ?? "";

        var azureClient = new AzureOpenAIClient(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey));

        _client = azureClient.GetChatClient(deployment);
    }

    public async Task<string> AskAsync(string prompt)
    {
        var requestOptions = new ChatCompletionOptions()
        {
            MaxOutputTokenCount = 500,
            Temperature = 0.7f,
            TopP = 0.9f,
            FrequencyPenalty = 0.2f,
            PresencePenalty = 0.2f,
        };
        List<ChatMessage> messages = new List<ChatMessage>()
        {
            new SystemChatMessage("Tu eres un asistente de ayuda. Responde únicamente en JSON válido."),
            new UserChatMessage(prompt),
        };
        var response = await _client.CompleteChatAsync(messages, requestOptions);

        return response.Value.Content[0].Text;
    }
}
