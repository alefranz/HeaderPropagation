[![Build Status](https://alefranz.visualstudio.com/HeaderPropagation/_apis/build/status/Build?branchName=master)](https://alefranz.visualstudio.com/HeaderPropagation/_build/latest?definitionId=3&branchName=master) [![](https://img.shields.io/nuget/v/HeaderPropagation.svg)](https://www.nuget.org/packages/HeaderPropagation/)

## About HeaderPropagation

This is a backport to ASP.NET Core 2.1 (and 2.2) of the
[HeaderPropagation middleware](https://github.com/aspnet/AspNetCore/pull/7921) I had recently contributed to the
[ASP.NET Core project](https://github.com/aspnet/AspNetCore).
All code is licensed under the Apache License, Version 2.0 and copyrighted by the [.NET Foundation](https://dotnetfoundation.org/).

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
services.AddHeaderPropagation(o => o.Headers.Add("User-Agent", new HeaderPropagationEntry
{
    DefaultValue = "Mozilla/5.0 (trust me, I'm really Mozilla!)",
}));
```
If you are using the `HttpClientFactory`, add the `DelegatingHandler` to the client configuration using the `AddHeaderPropagation` extension method.

```csharp
services.AddHttpClient<GitHubClient>(c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
}).AddHeaderPropagation();
```

See [samples/WebApplication](samples/WebApplication).

## Behaviour

`HeaderPropagationOptions` contains a dictionary where the key represent the name of the header to consume from the incoming request.

Each entry define the behaviour to propagate that header as follows:

- `OutboundHeaderName` determines the name of the header to be used for the outbound http requests.

- When present, the `ValueFactory` is the only method used to set the value. The factory should return `StringValues.Empty` to not add the header.

- When not present, the value will be taken from the header in the incoming request named as the key of this
entry in `HeaderPropagationOptions.Headers` or, if missing or empty, it will be the values
specified in `DefaultValue` or, if the `DefaultValue` is empty, the header will not
be added to the outbound calls.

Please note the factory is called only once per incoming request and the same value will be used by all the
outbound calls.