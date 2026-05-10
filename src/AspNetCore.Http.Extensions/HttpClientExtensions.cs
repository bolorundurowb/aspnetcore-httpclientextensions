using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Http.Extensions;

/// <summary>
/// JSON helpers for <see cref="HttpClient"/> and <see cref="HttpContent"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Serializes <paramref name="data"/> as JSON and sends a POST request.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="httpClient">The client used to send the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="data">The value to serialize as the request body.</param>
    /// <param name="jsonSerializerOptions">Options controlling serialization behavior.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(
        this HttpClient httpClient,
        string requestUri,
        T data,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateJsonContent(data, jsonSerializerOptions);
        return httpClient.PostAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Serializes <paramref name="data"/> as JSON and sends a PUT request.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="httpClient">The client used to send the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="data">The value to serialize as the request body.</param>
    /// <param name="jsonSerializerOptions">Options controlling serialization behavior.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(
        this HttpClient httpClient,
        string requestUri,
        T data,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateJsonContent(data, jsonSerializerOptions);
        return httpClient.PutAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Serializes <paramref name="data"/> as JSON and sends a PATCH request.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="httpClient">The client used to send the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="data">The value to serialize as the request body.</param>
    /// <param name="jsonSerializerOptions">Options controlling serialization behavior.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The HTTP response message.</returns>
    public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(
        this HttpClient httpClient,
        string requestUri,
        T data,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        var content = CreateJsonContent(data, jsonSerializerOptions);
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };
        return httpClient.SendAsync(request, cancellationToken);
    }

    /// <summary>
    /// Sends a GET request and deserializes the response body from JSON.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the content to.</typeparam>
    /// <param name="httpClient">The client used to send the request.</param>
    /// <param name="requestUri">The URI the request is sent to.</param>
    /// <param name="jsonSerializerOptions">Options controlling deserialization behavior.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized value, or <c>null</c> if the payload is JSON <c>null</c>.</returns>
    /// <exception cref="HttpRequestException">Thrown when the response status code does not indicate success.</exception>
    public static async Task<T?> GetFromJsonAsync<T>(
        this HttpClient httpClient,
        string requestUri,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient
            .GetAsync(requestUri, cancellationToken)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content
            .ReadAsJsonAsync<T>(jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the HTTP content as a stream and deserializes JSON, avoiding buffering the entire body as a string.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the content to.</typeparam>
    /// <param name="content">The HTTP content to read from.</param>
    /// <param name="jsonSerializerOptions">Options controlling deserialization behaviour.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The deserialized value, or <c>null</c> if the payload is JSON <c>null</c>.</returns>
    public static async Task<T?> ReadAsJsonAsync<T>(
        this HttpContent content,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
        return await JsonSerializer
            .DeserializeAsync<T>(stream, jsonSerializerOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    private static StringContent CreateJsonContent<T>(T data, JsonSerializerOptions? jsonSerializerOptions)
    {
        var dataAsString = JsonSerializer.Serialize(data, jsonSerializerOptions);
        return new StringContent(dataAsString, Encoding.UTF8, "application/json");
    }
}
