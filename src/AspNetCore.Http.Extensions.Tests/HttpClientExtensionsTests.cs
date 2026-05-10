using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
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
        handler.When(HttpMethod.Post, url).Respond(() => Task.FromResult(response));
        var client = new HttpClient(handler);

        var result = await client.PostAsJsonAsync(url, jsonBody, cancellationToken: TestContext.CurrentContext.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.CurrentContext.CancellationToken);
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
        handler.When(HttpMethod.Put, url).Respond(() => Task.FromResult(response));
        var client = new HttpClient(handler);

        var result = await client.PutAsJsonAsync(url, jsonBody, cancellationToken: TestContext.CurrentContext.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.CurrentContext.CancellationToken);
        body.Should().Be(jsonBody);
    }

    [Test]
    public async Task PatchAsJsonAsync_WhenHandlerReturnsContent_ReturnsResponseBody()
    {
        const string url = "https://example.com/api";
        var handler = new MockHttpMessageHandler();
        var response = new HttpResponseMessage();
        var jsonBody = JsonSerializer.Serialize(new SamplePayload { Id = 1, Name = "Test" });
        response.Content = new StringContent(jsonBody);
        handler.When(new HttpMethod("PATCH"), url).Respond(() => Task.FromResult(response));
        var client = new HttpClient(handler);

        var result = await client.PatchAsJsonAsync(url, jsonBody, cancellationToken: TestContext.CurrentContext.CancellationToken);

        var body = await result.Content.ReadAsStringAsync(TestContext.CurrentContext.CancellationToken);
        body.Should().Be(jsonBody);
    }

    [Test]
    public async Task GetFromJsonAsync_WhenResponseIsJson_ReturnsDeserializedObject()
    {
        const string url = "https://example.com/api";
        var expected = new SamplePayload { Id = 1, Name = "Test" };
        var json = JsonSerializer.Serialize(expected);
        var handler = new MockHttpMessageHandler();
        handler.When(HttpMethod.Get, url).Respond(
            HttpStatusCode.OK,
            new StringContent(json, Encoding.UTF8, "application/json"));
        var client = new HttpClient(handler);

        var result = await client.GetFromJsonAsync<SamplePayload>(url, cancellationToken: TestContext.CurrentContext.CancellationToken);

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task GetFromJsonAsync_WhenCamelCaseJson_UsesSerializerOptions()
    {
        const string url = "https://example.com/api";
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
        };
        var expected = new SamplePayload { Id = 2, Name = "Camel" };
        var json = JsonSerializer.Serialize(expected, options);
        var handler = new MockHttpMessageHandler();
        handler.When(HttpMethod.Get, url).Respond(
            HttpStatusCode.OK,
            new StringContent(json, Encoding.UTF8, "application/json"));
        var client = new HttpClient(handler);

        var result = await client.GetFromJsonAsync<SamplePayload>(url, options, TestContext.CurrentContext.CancellationToken);

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task ReadAsJsonAsync_WhenContentIsJson_ReturnsDeserializedObject()
    {
        var expected = new SamplePayload { Id = 1, Name = "Test" };
        var content = new StringContent(
            JsonSerializer.Serialize(expected),
            Encoding.UTF8,
            "application/json");

        var result = await content.ReadAsJsonAsync<SamplePayload>(cancellationToken: TestContext.CurrentContext.CancellationToken);

        result.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task ReadAsJsonAsync_WhenBodyIsLargeStream_Deserializes()
    {
        var payload = new LargePayload { Values = Enumerable.Range(0, 10_000).ToArray() };
        var json = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes(json);
        var stream = new MemoryStream(bytes, writable: false);
        using var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var result = await content.ReadAsJsonAsync<LargePayload>(cancellationToken: TestContext.CurrentContext.CancellationToken);

        result.Should().NotBeNull();
        result!.Values.Should().HaveCount(10_000);
        result.Values[9999].Should().Be(9999);
    }

    [Test]
    public async Task GetFromJsonAsync_WhenAlreadyCanceled_ThrowsOperationCanceledException()
    {
        const string url = "https://example.com/api";
        var handler = new MockHttpMessageHandler();
        handler.When(HttpMethod.Get, url).Respond(HttpStatusCode.OK, new StringContent("{}", Encoding.UTF8, "application/json"));
        var client = new HttpClient(handler);
        var token = new CancellationToken(true);

        var act = async () => await client.GetFromJsonAsync<SamplePayload>(url, cancellationToken: token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    private sealed class SamplePayload
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    private sealed class LargePayload
    {
        public int[] Values { get; set; } = null!;
    }
}
