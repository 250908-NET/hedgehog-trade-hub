using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TradeHub.Api;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Services.Interfaces;
using TradeHub.Api.Utilities;
using TradeHub.Test.Helpers;

namespace TradeHub.Test.Integration;

public class ItemControllerMockedEndpointsTests(WebApplicationFactory<Program> factory)
    : IntegrationTestBase(factory)
{
    #region GET /items

    [Fact]
    public async Task GetAllItems_ShouldReturnOkAndListOfItems_WhenItemsExist()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        var expectedDtos = new List<ItemDTO>
        {
            new(1, "Item 1", "Desc 1", "", 10, 1, "", Condition.New, Availability.Available),
            new(2, "Item 2", "Desc 2", "", 20, 1, "", Condition.UsedGood, Availability.Available),
        };
        mockItemService.Setup(s => s.GetAllItemsAsync()).ReturnsAsync(expectedDtos);

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.GetAsync("/items");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualDtos = await response.Content.ReadFromJsonAsync<List<ItemDTO>>(_jsonOptions);
        Assert.NotNull(actualDtos);
        Assert.Equal(2, actualDtos.Count);
    }

    #endregion

    #region GET /items/{id}

    [Fact]
    public async Task GetItemById_ShouldReturnOkAndItem_WhenItemExists()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        var expectedDto = new ItemDTO(
            1,
            "Test Item",
            "Desc",
            "",
            50,
            1,
            "",
            Condition.New,
            Availability.Available
        );
        mockItemService.Setup(s => s.GetItemByIdAsync(1)).ReturnsAsync(expectedDto);

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.GetAsync("/items/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualDto = await response.Content.ReadFromJsonAsync<ItemDTO>(_jsonOptions);
        Assert.NotNull(actualDto);
        Assert.Equal(expectedDto.Name, actualDto.Name);
    }

    [Fact]
    public async Task GetItemById_ShouldReturnNotFound_WhenServiceThrowsNotFound()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        mockItemService
            .Setup(s => s.GetItemByIdAsync(999))
            .ThrowsAsync(new NotFoundException("Item not found"));

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.GetAsync("/items/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region POST /items

    [Fact]
    public async Task CreateItem_ShouldReturnOkAndCreatedItem_WhenDtoIsValid()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        var createDto = new CreateItemDTO(
            "New Item",
            "Desc",
            "",
            100,
            1,
            "",
            Condition.New,
            Availability.Available
        );
        var expectedDto = new ItemDTO(
            1,
            "New Item",
            "Desc",
            "",
            100,
            1,
            "",
            Condition.New,
            Availability.Available
        );
        mockItemService.Setup(s => s.CreateItemAsync(createDto)).ReturnsAsync(expectedDto);

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.PostAsJsonAsync("/items", createDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualDto = await response.Content.ReadFromJsonAsync<ItemDTO>(_jsonOptions);
        Assert.NotNull(actualDto);
        Assert.Equal(expectedDto.Id, actualDto.Id);
    }

    #endregion

    #region PUT /items/{id}

    [Fact]
    public async Task UpdateItem_ShouldReturnOkAndUpdatedItem_WhenSuccessful()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        var updateDto = new UpdateItemDTO("Updated Name", null, null, null, null, null, null, null);
        var expectedDto = new ItemDTO(
            1,
            "Updated Name",
            "Desc",
            "",
            100,
            1,
            "",
            Condition.New,
            Availability.Available
        );
        mockItemService.Setup(s => s.UpdateItemAsync(1, updateDto)).ReturnsAsync(expectedDto);

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.PutAsJsonAsync("/items/1", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var actualDto = await response.Content.ReadFromJsonAsync<ItemDTO>(_jsonOptions);
        Assert.NotNull(actualDto);
        Assert.Equal("Updated Name", actualDto.Name);
    }

    [Fact]
    public async Task UpdateItem_ShouldReturnNotFound_WhenServiceThrowsNotFound()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        var updateDto = new UpdateItemDTO("Updated Name", null, null, null, null, null, null, null);
        mockItemService
            .Setup(s => s.UpdateItemAsync(999, updateDto))
            .ThrowsAsync(new NotFoundException("Item not found"));

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.PutAsJsonAsync("/items/999", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateItem_ShouldReturnConflict_WhenServiceThrowsConflict()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        var updateDto = new UpdateItemDTO("Updated Name", null, null, null, null, null, null, null);
        mockItemService
            .Setup(s => s.UpdateItemAsync(1, updateDto))
            .ThrowsAsync(new ConflictException("Concurrency conflict"));

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.PutAsJsonAsync("/items/1", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    #endregion

    #region DELETE /items/{id}

    [Fact]
    public async Task DeleteItem_ShouldReturnOkAndTrue_WhenSuccessful()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        mockItemService.Setup(s => s.DeleteItemAsync(1)).ReturnsAsync(true);

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.DeleteAsync("/items/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<bool>();
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteItem_ShouldReturnNotFound_WhenServiceThrowsNotFound()
    {
        // Arrange
        var mockItemService = new Mock<IItemService>();
        mockItemService
            .Setup(s => s.DeleteItemAsync(999))
            .ThrowsAsync(new NotFoundException("Item not found"));

        var client = CreateTestClient(services =>
        {
            services.Replace(ServiceDescriptor.Singleton(mockItemService.Object));
        });

        // Act
        var response = await client.DeleteAsync("/items/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion
}
