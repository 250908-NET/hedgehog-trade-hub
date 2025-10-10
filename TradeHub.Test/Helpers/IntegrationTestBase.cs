using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TradeHub.API;
using TradeHub.API.Models;
using TradeHub.API.Utilities;

namespace TradeHub.Test.Helpers;

/// <summary>
/// Base class for integration tests, providing common setup like an in-memory database and mocked services.
/// </summary>
public abstract class IntegrationTestBase(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> _factory = factory;

    protected static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new FlexibleEnumConverterFactory() },
    };

    /// <summary>
    /// Create a test HttpClient with a mocked service and in-memory database.
    /// </summary>
    protected HttpClient CreateTestClient(
        Action<IServiceCollection>? testServicesConfiguration = null
    )
    {
        return _factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    // silence logging during tests
                    services.RemoveAll<ILoggerFactory>();
                    services.AddSingleton<ILoggerFactory>(new NullLoggerFactory());

                    // mock specific services for the test as necessary
                    testServicesConfiguration?.Invoke(services);

                    // mock real DbContext using new in-memory database
                    services.RemoveAll<DbContextOptions<TradeHubContext>>();
                    services.AddDbContext<TradeHubContext>(options =>
                    {
                        options.UseInMemoryDatabase($"InMemoryTestDb_{Guid.NewGuid()}");
                    });
                });
            })
            .CreateClient();
    }
}
