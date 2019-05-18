[![Build Status](https://alefranz.visualstudio.com/HeaderPropagation/_apis/build/status/Build?branchName=master)](https://alefranz.visualstudio.com/HeaderPropagation/_build/latest?definitionId=3&branchName=master) [![](https://img.shields.io/nuget/v/HeaderPropagation.svg)](https://www.nuget.org/packages/HeaderPropagation/)

## About HeaderPropagation

This is a backport to ASP.NET Core 2.1 (and 2.2) of the
[HeaderPropagation middleware](https://github.com/aspnet/AspNetCore/pull/7921) I had recently contributed to the
[ASP.NET Core](https://github.com/aspnet/AspNetCore) project.
All code is licensed under the Apache License, Version 2.0 and copyrighted by the [.NET Foundation](https://dotnetfoundation.org/).

If you are using ASP.NET Core 3.0, please use the official package [Microsoft.AspNetCore.HeaderPropagation](https://www.nuget.org/packages/Microsoft.AspNetCore.HeaderPropagation/).

## Motivation
I believe it is a common use case which deserves to be included in ASP.NET Core.
Its main use case is probably to track distributed transaction which requires the ability to pass through a transaction identifier as well as generating a new one when not present.

Given the ASP.NET Core 3.0 release is quite far away, and the current policy doesn't allow to backport new features to already shipped releases, I have created this package as [recommended](https://github.com/aspnet/AspNetCore/pull/7921#issuecomment-479717164) so it can be used today on projects based on ASP.NET Core 2.1 or 2.2.

## Usage

In `Startup.Configure` enable the middleware:

```csharp
app.UseHeaderPropagation();
```

In `Startup.ConfigureServices` add the required services, eventually specifying a configuration action:

```csharp
services.AddHeaderPropagation(o =>
{
    // propagate the header if present
    o.Headers.Add("User-Agent");

    // if still missing, set it with a value factory
    o.Headers.Add("User-Agent", context => "Mozilla/5.0 (trust me, I'm really Mozilla!)");

    // propagate the header if present, using a different name in the outbound request
    o.Headers.Add("Accept-Language", "Lang");
});
```

If you are using the `HttpClientFactory`, add the `DelegatingHandler` to the client configuration using the `AddHeaderPropagation` extension method.

```csharp
services.AddHttpClient<GitHubClient>(c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
}).AddHeaderPropagation();
```

Or propagate only a specific header, also redefining the name to use

```csharp
services.AddHttpClient("example", c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
}).AddHeaderPropagation(o =>
{
    o.Headers.Add("User-Agent", "Source");
});
```

See [samples/WebApplication](samples/WebApplication).

## Behaviour

`HeaderPropagationOptions` contains a dictionary where the key represent the name of the header to consume from the incoming request.

Each entry define the behaviour to propagate that header as follows:

- `InboundHeaderName` is the name of the header to be captured.
- `CapturedHeaderName` determines the name of the header to be used by default for the outbound http requests. If not specified, defaults to `InboundHeaderName`.
- When present, the `ValueFilter` delegate will be evaluated once per request to provide the transformed
header value. The delegate will be called regardless of whether a header with the name corresponding to `InboundHeaderName` is present in the request. It should return `StringValues.Empty` to not add the header.
- If multiple configurations for the same header are present, the first which returns a value wins.

Please note the factory is called only once per incoming request and the same value will be used by all the
outbound calls.

`HeaderPropagationMessageHandlerOptions` allows to customize the behaviour per clients, where each entry define the behaviour as follows:

- `CapturedHeaderName` is the name of the header to be used to lookup the headers captured.
- `OutboundHeaderName` is the name of the header to be added to the outgoing http requests. If not specified, defaults to `CapturedHeaderName`.

# Acknowledgements

This feature would not have been possible without the help of [@rynowak](https://github.com/rynowak) who helped to refine it and get it merged into [ASP.NET Core](https://github.com/aspnet/AspNetCore).

You can find the [list of contributions](https://github.com/aspnet/AspNetCore/commits/master/src/Middleware/HeaderPropagation) in the original repository.
