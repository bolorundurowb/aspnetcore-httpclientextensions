# aspnetcore-httpclientextensions

[![CircleCI](https://circleci.com/gh/bolorundurowb/aspnetcore-httpclientextensions.svg?style=svg)](https://circleci.com/gh/bolorundurowb/aspnetcore-httpclientextensions)

## About ASP.NET Core HttpClient Extensions

Adds extension methods that allow you pass strongly typed objects as parameters to the `HttpClient` instance.

## Usage

To make use of this library, add it to your project from Nuget using any of the following methods.

```bash
PM> Install-Package AspNet.Http.Extensions
```

or

```bash
dotnet add package AspNet.Http.Extensions
```

or 

```bash
paket add AspNet.Http.Extensions
```

Add the appropriate namespace to your class

```csharp
using AspNet.Http.Extensions;
```

Create an instance of `HttpClient`

```csharp
var client = new HttpClient();
```

Create an instance of your payload class

```csharp
var payload = new Foo { Name = "Bar" };
```

Make your HTTP request

```csharp
var response = client.PostAsJsonAsync('url', payload);
```

## Other methods

the library also provides two other extension methods; `PutAsJsonAsync` and `ReadAsJsonAsync`. 