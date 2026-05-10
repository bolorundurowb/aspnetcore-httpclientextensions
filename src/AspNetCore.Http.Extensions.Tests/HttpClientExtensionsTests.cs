using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AwesomeAssertions;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace AspNetCore.Http.Extensions.Tests;

[TestFixture]
public sealed class HttpClientExtensionsTests
{
    [Test]
    public async Task PostAsJsonAsync_WhenHandlerReturnsContent_ReturnsResponseBody()
    {
        const string url = "https://example.com/api";
        var handler = new MockHttpMessageHandler();
        var response = new HttpResponseMessage();
        var jsonBody = JsonSerializer.Serialize(new SamplePayload { Id = 1, Name = "Test" });
        response.Content = new StringContent(jsonBody);
        handler.When(url).Respond(() => Task.FromResult(response));
        var client = new HttpClient(handler);

        var result = await client.PostAsJsonAsync(url, jsonBody);

        var body = await result.Content.ReadAsStringAsync();
        body.Should().Be(jsonBody);
    }

    [Test]
    public async Task PutAsJsonAsync_WhenHandlerReturnsContent_ReturnsResponseBody()
    {
        const string url = "https://example.com/api";
        var handler = new MockHttpMessageHandler();
        var response = new HttpResponseMessage();
        var jsonBody = JsonSerializer.Serialize(new SamplePayload { Id = 1, Name = "Test" });
        response.Content = new StringContent(jsonBody);
        handler.When(url).Respond(() => Task.FromResult(response));
        var client = new HttpClient(handler);

        var result = await client.PutAsJsonAsync(url, jsonBody);

        var body = await result.Content.ReadAsStringAsync();
        body.Should().Be(jsonBody);
    }

    [Test]
    public async Task ReadAsJsonAsync_WhenContentIsJson_ReturnsDeserializedObject()
    {
        var expected = new SamplePayload { Id = 1, Name = "Test" };
        var content = new StringContent(
            JsonSerializer.Serialize(expected),
            Encoding.UTF8,
            "application/json");

        var result = await content.ReadAsJsonAsync<SamplePayload>();

        result.Should().BeEquivalentTo(expected);
    }

    private sealed class SamplePayload
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
