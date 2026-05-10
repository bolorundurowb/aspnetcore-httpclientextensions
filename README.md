# aspnetcore-httpclientextensions

[![Build, Test & Coverage](https://github.com/bolorundurowb/aspnetcore-httpclientextensions/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/aspnetcore-httpclientextensions/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/aspnetcore-httpclientextensions/graph/badge.svg?token=39Y5TP2LIL)](https://codecov.io/gh/bolorundurowb/aspnetcore-httpclientextensions)  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) ![NuGet Version](https://img.shields.io/nuget/v/AspNetCore.Http.Extensions)

Small **System.Text.Json** helpers for **`HttpClient`** and **`HttpContent`**: send JSON with POST, PUT, or PATCH, deserialize JSON from responses or arbitrary content, with optional **`JsonSerializerOptions`** and **`CancellationToken`** support.

**Target framework:** [.NET Standard 2.0](https://learn.microsoft.com/dotnet/standard/net-standard) (broad compatibility across .NET Framework 4.6.1+, .NET Core 2.0+, Mono, Xamarin, Unity, and modern .NET).

## When to use this library

This package is aimed **mainly at runtimes and workloads from before the “one .NET” era** especially **.NET Core 3.1 and earlier**, **.NET Framework**, **Mono**, **Xamarin**, **Unity**, and other **non-unified .NET** environments where you want a **lightweight `HttpClient` + JSON** story without pulling in a large stack.

Starting with **.NET 5**, the shared runtime ships **[`System.Net.Http.Json`](https://learn.microsoft.com/dotnet/api/system.net.http.json)** (`GetFromJsonAsync`, `PostAsJsonAsync`, `PutAsJsonAsync`, `ReadFromJsonAsync`, and related types). **If you target .NET 5 or later**, prefer that built-in API (or the standalone `System.Net.Http.Json` package where applicable) unless you have a concrete reason to standardize on this library.

This project remains useful when you need **.NET Standard 2.0**-friendly extensions built on **`System.Text.Json`** with a **small surface area** and **stream-based** deserialization for large bodies (`ReadAsJsonAsync` on `HttpContent`).

## Installation

From the [NuGet package **AspNetCore.Http.Extensions**](https://www.nuget.org/packages/AspNetCore.Http.Extensions):

```bash
dotnet add package AspNetCore.Http.Extensions
```

**Package Manager Console:**

```powershell
Install-Package AspNetCore.Http.Extensions
```

**Paket:**

```bash
paket add AspNetCore.Http.Extensions
```

Add the namespace:

```csharp
using AspNetCore.Http.Extensions;
```

## Usage

Create an `HttpClient` (often via `IHttpClientFactory` in ASP.NET Core):

```csharp
public sealed class Foo
{
    public string Name { get; set; } = "";
}
```

```csharp
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;

var client = new HttpClient();
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var cts = new CancellationTokenSource();

var payload = new Foo { Name = "Bar" };

// POST / PUT / PATCH JSON (returns HttpResponseMessage)
var postResponse = await client.PostAsJsonAsync("https://api.example.com/items", payload, options, cts.Token);
var putResponse = await client.PutAsJsonAsync("https://api.example.com/items/1", payload, options, cts.Token);
var patchResponse = await client.PatchAsJsonAsync("https://api.example.com/items/1", payload, options, cts.Token);

// GET and deserialize JSON (throws if the status is not success)
var item = await client.GetFromJsonAsync<Foo>("https://api.example.com/items/1", options, cts.Token);

// Deserialize from HttpContent (uses a stream; suitable for large payloads)
var fromContent = await postResponse.Content.ReadAsJsonAsync<Foo>(options, cts.Token);
```

`JsonSerializerOptions` and `CancellationToken` are optional on every extension method; omit them when you do not need custom serialization or cancellation.

## API overview

| Method | Description |
|--------|-------------|
| `PostAsJsonAsync` | Serialize body as JSON, **POST** |
| `PutAsJsonAsync` | Serialize body as JSON, **PUT** |
| `PatchAsJsonAsync` | Serialize body as JSON, **PATCH** (via `SendAsync`; `HttpClient.PatchAsync` is not available on .NET Standard 2.0) |
| `GetFromJsonAsync` | **GET**, ensure success, deserialize JSON |
| `ReadAsJsonAsync` | Deserialize JSON from **`HttpContent`** using a **stream** |

XML documentation is included in the NuGet package for IntelliSense.

## License

See [LICENSE](LICENSE) in the repository.
