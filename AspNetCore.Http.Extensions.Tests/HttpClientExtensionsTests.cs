using FluentAssertions;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace AspNetCore.Http.Extensions.Tests;

[TestFixture]
public class HttpClientExtensionsTests
{
    [Test]
    public async Task PostAsJsonAsync_ShouldSerializeDataAndSetContentTypeHeader()
    {
        // Arrange
        const string expectedUrl = "https://example.com/api";
        var messageHandler = new MockHttpMessageHandler();
        var response = new HttpResponseMessage();
        var expectedData = JsonSerializer.Serialize(new TestData { Id = 1, Name = "Test" });
        response.Content = new StringContent(expectedData);
        messageHandler.When(expectedUrl).Respond(() => Task.FromResult(response));

        var httpClient = new HttpClient(messageHandler);

        // Act
        var result = await httpClient.PostAsJsonAsync(expectedUrl, expectedData);

        // Assert
        var data = await result.Content.ReadAsStringAsync();

        data.Should().Be(expectedData);
    }

    [Test]
    public async Task PutAsJsonAsync_ShouldSerializeDataAndSetContentTypeHeader()
    {
        // Arrange
        const string expectedUrl = "https://example.com/api";
        var messageHandler = new MockHttpMessageHandler();
        var response = new HttpResponseMessage();
        var expectedData = JsonSerializer.Serialize(new TestData { Id = 1, Name = "Test" });
        response.Content = new StringContent(expectedData);
        messageHandler.When(expectedUrl).Respond(() => Task.FromResult(response));

        var httpClient = new HttpClient(messageHandler);

        // Act
        var result = await httpClient.PutAsJsonAsync(expectedUrl, expectedData);

        // Assert
        var data = await result.Content.ReadAsStringAsync();

        data.Should().Be(expectedData);
    }

    [Test]
    public async Task ReadAsJsonAsync_ShouldDeserializeContentData()
    {
        // Arrange
        var expectedData = new TestData { Id = 1, Name = "Test" };
        var content = new StringContent(JsonSerializer.Serialize(expectedData), Encoding.UTF8, "application/json");

        // Act
        var result = await content.ReadAsJsonAsync<TestData>();

        // Assert
        result.Should().BeEquivalentTo(expectedData);
    }
}

public class TestData
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
