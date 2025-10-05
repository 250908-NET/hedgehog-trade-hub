using System.Text.Json;
using System.Text.Json.Serialization;
using TradeHub.Api.Services.Interfaces;
using TradeHub.Api.Utilities;

namespace TradeHub.Api.Services;

public class MultiLLMService(
    HttpClient httpClient,
    ILogger<MultiLLMService> logger,
    IConfiguration configuration
) : ILLMService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<MultiLLMService> _logger = logger;
    private readonly string? _geminiApiKey = configuration["GEMINI_API_KEY"];
    private readonly string? _openAIApiKey = configuration["OPENAI_API_KEY"];

    public async Task<decimal> EstimateItemValueAsync(
        string itemName,
        string itemDescription,
        string itemCondition,
        string currency = "USD"
    )
    {
        _logger.LogInformation("Estimating value of item {ItemName}...", itemName);

        string prompt =
            $"Estimate the value of an item named '{itemName}' with description '{itemDescription}' in the condition '{itemCondition}' in {currency}. "
            + "Respond with a single number without money symbol or any other text.";

        decimal value = 0;
        bool success = false;

        if (_geminiApiKey != null)
        {
            success = (await CallGeminiAsync(prompt)).SafeParseMoney(out value, true, true);
        }
        else
        {
            _logger.LogWarning("GEMINI_API_KEY not found, skipping.");
        }

        if (_openAIApiKey != null && !success)
        {
            success = (await CallOpenAIAsync(prompt)).SafeParseMoney(out value, true, true);
        }
        else
        {
            _logger.LogWarning("OPENAI_API_KEY not found, skipping.");
        }

        if (!success)
        {
            throw new Exception("Failed to estimate item value.");
        }

        _logger.LogInformation("Estimated item value is {Value}!", value);
        return value;
    }

    /// <summary>
    /// Call the Google Gemini API with the given prompt.
    /// </summary>
    /// <param name="prompt">The prompt to send to the LLM</param>
    /// <returns>The response from the LLM, as a string</returns>
    /// <remarks>
    /// Example request:
    /// <code>
    /// curl "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent" \
    ///   -H "x-goog-api-key: $GEMINI_API_KEY" \
    ///   -H 'Content-Type: application/json' \
    ///   -X POST \
    ///   -d '{
    ///     "contents": [
    ///       {
    ///         "parts": [
    ///           {
    ///             "text": "Explain how AI works in a few words"
    ///           }
    ///         ]
    ///       }
    ///     ]
    ///     "generationConfig": {
    ///       "thinkingConfig": {
    ///         "thinkingBudget": 0
    ///       }
    ///     }
    ///   }'
    /// <code>
    /// </remarks>
    internal async Task<string> CallGeminiAsync(string prompt)
    {
        _logger.LogInformation("Calling Gemini API...");

        string model = "gemini-2.5-flash";
        string url =
            $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";

        using HttpRequestMessage request = new(HttpMethod.Post, url);

        request.Headers.Add("Content-Type", "application/json");
        request.Headers.Add("x-goog-api-key", _geminiApiKey);

        var requestBody = new
        {
            contents = new { parts = new { text = prompt } },
            generationConfig = new
            {
                thinkingConfig = new
                {
                    thinkingBudget = 0, // disable thinking
                },
            },
        };
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody));

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            throw new InternalServerException(
                $"Gemini API returned non-success status code: {response.StatusCode}"
            );
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        GeminiResponse response_data = JsonSerializer.Deserialize<GeminiResponse>(responseBody)!;

        _logger.LogInformation(
            "Gemini API returned response, {TokenCount} total tokens used.",
            response_data.UsageMetadata?.TotalTokenCount ?? 0
        );

        return response_data.Candidates?[0].Parts?[0].Text ?? "";
    }

    internal record GeminiResponse(
        [property: JsonPropertyName("candidates")] List<Candidate>? Candidates,
        [property: JsonPropertyName("usageMetadata")] UsageMetadata? UsageMetadata
    );

    internal record Candidate([property: JsonPropertyName("parts")] List<Part>? Parts);

    internal record Part([property: JsonPropertyName("text")] string? Text);

    internal record UsageMetadata(
        [property: JsonPropertyName("totalTokenCount")] int TotalTokenCount
    );

    internal async Task<string> CallOpenAIAsync(string prompt)
    {
        throw new NotImplementedException();
    }
}
