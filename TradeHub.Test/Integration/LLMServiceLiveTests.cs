using DotNetEnv;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TradeHub.Api.Services;
using TradeHub.Api.Utilities;
using Xunit;

namespace TradeHub.Test.Services;

// This class contains "live" tests that make actual external API calls.
// They are skipped if API keys are not configured.
public class MultiLLMServiceLiveTests
{
    private readonly MultiLLMService _llmService;
    private readonly string? _geminiApiKey;
    private readonly string? _openAIApiKey;

    public MultiLLMServiceLiveTests()
    {
        // Ensure .env file is loaded for live tests
        Env.Load();

        var mockLogger = new Mock<ILogger<MultiLLMService>>();

        // Build configuration to read environment variables
        var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

        _geminiApiKey = configuration["GEMINI_API_KEY"];
        _openAIApiKey = configuration["OPENAI_API_KEY"];

        // Use a real HttpClient for live tests
        var httpClient = new HttpClient();

        _llmService = new MultiLLMService(httpClient, mockLogger.Object, configuration);
    }

    #region Live Gemini Tests

    [SkippableFact]
    public async Task CallGeminiAsync_ShouldReturnValidResponse_WhenApiKeyExists()
    {
        // Arrange
        Skip.If(string.IsNullOrEmpty(_geminiApiKey));

        string prompt = "What is the capital of France? Respond with only the city name.";

        // Act
        string response = await _llmService.CallGeminiAsync(prompt);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(response));
        Assert.Contains("Paris", response, StringComparison.OrdinalIgnoreCase);
    }

    [SkippableFact]
    public async Task EstimateItemValueAsync_ShouldReturnValidDecimalFromGemini_WhenGeminiKeyExists()
    {
        // Arrange
        Skip.If(string.IsNullOrEmpty(_geminiApiKey));

        string itemName = "Vintage Rolex Submariner";
        string itemDescription = "A classic diver's watch from the 1970s, good condition.";
        string itemCondition = "UsedGood";

        // Act
        decimal estimatedValue = await _llmService.EstimateItemValueAsync(
            itemName,
            itemDescription,
            itemCondition
        );

        // Assert
        Assert.True(
            estimatedValue > 0,
            $"Expected estimated value to be greater than 0, but got {estimatedValue}"
        );
        // You might add more specific range checks if you have expectations, e.g., Assert.InRange(estimatedValue, 5000m, 20000m);
    }

    #endregion

    #region Live OpenAI Tests

    [SkippableFact(typeof(NotImplementedException))]
    public async Task CallOpenAIAsync_ShouldReturnValidResponse_WhenApiKeyExists()
    {
        // Arrange
        Skip.If(string.IsNullOrEmpty(_openAIApiKey));

        string prompt = "What is the capital of Germany? Respond with only the city name.";

        // Act
        string response = await _llmService.CallOpenAIAsync(prompt);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(response));
        Assert.Contains("Berlin", response, StringComparison.OrdinalIgnoreCase);
    }

    [SkippableFact(typeof(NotImplementedException))]
    public async Task EstimateItemValueAsync_ShouldReturnValidDecimalFromOpenAI_WhenOpenAIKeyExists()
    {
        // Arrange
        Skip.If(string.IsNullOrEmpty(_openAIApiKey));

        string itemName = "Rare First Edition Book";
        string itemDescription = "Signed by author, excellent condition, from 1850.";
        string itemCondition = "UsedLikeNew";

        // Act
        decimal estimatedValue = await _llmService.EstimateItemValueAsync(
            itemName,
            itemDescription,
            itemCondition
        );

        // Assert
        Assert.True(
            estimatedValue > 0,
            $"Expected estimated value to be greater than 0, but got {estimatedValue}"
        );
    }

    #endregion
}
