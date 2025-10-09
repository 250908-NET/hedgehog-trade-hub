using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using TradeHub.API.Services;

namespace TradeHub.Test.Services;

public class MultiLLMServiceTests
{
    private readonly Mock<HttpClient> _mockHttpClient;
    private readonly Mock<ILogger<MultiLLMService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<MultiLLMService> _mockLlmService; // Mock the service itself to mock internal methods

    public MultiLLMServiceTests()
    {
        _mockHttpClient = new Mock<HttpClient>();
        _mockLogger = new Mock<ILogger<MultiLLMService>>();
        _mockConfiguration = new Mock<IConfiguration>();

        // Create a mock of MultiLLMService to allow mocking of internal methods
        // Pass the mocked dependencies to its constructor
        _mockLlmService = new Mock<MultiLLMService>(
            _mockHttpClient.Object,
            _mockLogger.Object,
            _mockConfiguration.Object
        )
        {
            CallBase = true, // Important: Call base implementation for un-mocked methods
        };
    }

    private void SetupApiKeys(string? geminiKey, string? openAiKey)
    {
        _mockConfiguration.SetupGet(c => c["GEMINI_API_KEY"]).Returns(geminiKey);
        _mockConfiguration.SetupGet(c => c["OPENAI_API_KEY"]).Returns(openAiKey);
    }

    #region EstimateItemValueAsync Tests

    [Fact]
    public async Task EstimateItemValueAsync_ShouldReturnGeminiValue_WhenGeminiKeyExistsAndSucceeds()
    {
        // Arrange
        SetupApiKeys("GEMINI_KEY", null);
        _mockLlmService.Setup(s => s.CallGeminiAsync(It.IsAny<string>())).ReturnsAsync("123.45");
        _mockLlmService
            .Setup(s => s.CallOpenAIAsync(It.IsAny<string>()))
            .ThrowsAsync(new NotImplementedException()); // Should not be called

        // Act
        var result = await _mockLlmService.Object.EstimateItemValueAsync(
            "Test Item",
            "Description",
            "New"
        );

        // Assert
        Assert.Equal(123.45m, result);
        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Once);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EstimateItemValueAsync_ShouldFallbackToOpenAI_WhenGeminiFailsToParse()
    {
        // Arrange
        SetupApiKeys("GEMINI_KEY", "OPENAI_KEY");
        _mockLlmService
            .Setup(s => s.CallGeminiAsync(It.IsAny<string>()))
            .ReturnsAsync("Not a number"); // Gemini returns unparseable
        _mockLlmService.Setup(s => s.CallOpenAIAsync(It.IsAny<string>())).ReturnsAsync("67.89"); // OpenAI succeeds

        // Act
        var result = await _mockLlmService.Object.EstimateItemValueAsync(
            "Test Item",
            "Description",
            "New"
        );

        // Assert
        Assert.Equal(67.89m, result);
        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Once);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task EstimateItemValueAsync_ShouldFallbackToOpenAI_WhenGeminiCallThrowsException()
    {
        // Arrange
        SetupApiKeys("GEMINI_KEY", "OPENAI_KEY");
        _mockLlmService
            .Setup(s => s.CallGeminiAsync(It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("Gemini network error")); // Gemini throws
        _mockLlmService.Setup(s => s.CallOpenAIAsync(It.IsAny<string>())).ReturnsAsync("67.89"); // OpenAI succeeds

        // Act
        var result = await _mockLlmService.Object.EstimateItemValueAsync(
            "Test Item",
            "Description",
            "New"
        );

        // Assert
        Assert.Equal(67.89m, result);
        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Once);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task EstimateItemValueAsync_ShouldReturnOpenAIValue_WhenOnlyOpenAIKeyExistsAndSucceeds()
    {
        // Arrange
        SetupApiKeys(null, "OPENAI_KEY");
        _mockLlmService
            .Setup(s => s.CallGeminiAsync(It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException()); // Should not be called
        _mockLlmService.Setup(s => s.CallOpenAIAsync(It.IsAny<string>())).ReturnsAsync("50.00");

        // Act
        var result = await _mockLlmService.Object.EstimateItemValueAsync(
            "Test Item",
            "Description",
            "New"
        );

        // Assert
        Assert.Equal(50.00m, result);
        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Never);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task EstimateItemValueAsync_ShouldThrowException_WhenBothFailToParse()
    {
        // Arrange
        SetupApiKeys("GEMINI_KEY", "OPENAI_KEY");
        _mockLlmService
            .Setup(s => s.CallGeminiAsync(It.IsAny<string>()))
            .ReturnsAsync("Invalid Gemini");
        _mockLlmService
            .Setup(s => s.CallOpenAIAsync(It.IsAny<string>()))
            .ReturnsAsync("Invalid OpenAI");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _mockLlmService.Object.EstimateItemValueAsync("Test Item", "Description", "New")
        );

        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Once);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task EstimateItemValueAsync_ShouldThrowException_WhenNoApiKeysExist()
    {
        // Arrange
        SetupApiKeys(null, null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _mockLlmService.Object.EstimateItemValueAsync("Test Item", "Description", "New")
        );

        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Never);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EstimateItemValueAsync_ShouldThrowException_WhenGeminiFailsAndNoOpenAIKey()
    {
        // Arrange
        SetupApiKeys("GEMINI_KEY", null);
        _mockLlmService
            .Setup(s => s.CallGeminiAsync(It.IsAny<string>()))
            .ReturnsAsync("Not a number");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _mockLlmService.Object.EstimateItemValueAsync("Test Item", "Description", "New")
        );

        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Once);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task EstimateItemValueAsync_ShouldThrowException_WhenOpenAIFailsAndNoGeminiKey()
    {
        // Arrange
        SetupApiKeys(null, "OPENAI_KEY");
        _mockLlmService
            .Setup(s => s.CallOpenAIAsync(It.IsAny<string>()))
            .ReturnsAsync("Not a number");

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _mockLlmService.Object.EstimateItemValueAsync("Test Item", "Description", "New")
        );

        _mockLlmService.Verify(s => s.CallGeminiAsync(It.IsAny<string>()), Times.Never);
        _mockLlmService.Verify(s => s.CallOpenAIAsync(It.IsAny<string>()), Times.Once);
    }

    #endregion
}
