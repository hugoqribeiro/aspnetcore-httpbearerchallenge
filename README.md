# HTTP Bearer Challenge with ASP.NET Core

This sample illustrates how to build a complete `WWW-Authenticate` header as part of an HTTP Bearer Challenge with ASP.NET Core.

## Concept

The OAuth2 specification ([RFC 6749](https://datatracker.ietf.org/doc/html/rfc6749)) specifies, on section 5.2, that the authorization server should return a `WWW-Authenticate` header when the client authentication fails (`401 Unauthorized`).

The HTTP Authentication specification ([RFC 7235](https://datatracker.ietf.org/doc/html/rfc7235)), on the other hand, describes, on section 4.1, how this `WWW-Authenticate` header should be built with at least a challenge.

Example:

```
WWW-Authenticate: Bearer realm="https://server.com/trucks", authorization_uri="https://authorizationserver.com", audience="rest-api", scope="read-trucks"
```

When using ASP.NET and JWT-bearer authentication to secure a REST API, by default, the authorization middleware will produce the `WWW-Authenticate` header for unauthorized requests like this:

```
WWW-Authenticate: Bearer
```

So this makes it impossible for client applications to know anything about what they need to do to have their requests authorized other than knowing that they need to provider a valid bearer token.

> If the response were to be like the example above, the client application would know where is the authorization server and the audience and scope(s) required.

So, this sample, illustrates how to customize the behavior of the JWT Bearer authorization handler to have it produce a complete and dynamic HTTP Bearer challenge.

## HttpBearerChallenge Library

This library provides all the pieces required to achieve this:

### 1 . `HttpBearerChallengeBuilder`

This static class allows building the `WWW-Authenticate` header value from an instance of `JwtBearerChallengeContext` and an optional scope.

> The `JwtBearerChallengeContext` instance is obtained in `HttpBearerChallengeEvents`.

### 2. `HttpBearerChallengeEvents`

This type is an extension of `JwtBearerEvents` (that allows handling events raised by the authorization handler) that implements the `Challenge()` event and constructs the header value for each unauthorized request.

### 3. `AuthorizationPolicyBuilderExtensions`

This class provides an extension method for `AuthorizationPolicyBuilder` - `RequireScope()` - that sets up an authorization policy to add the `ScopeAuthorizationRequirement` requirement, which has 2 responsibilities:

1. Validating that requests to any given API endpoint require a valid access token, containing the specified scope(s).

2. Adding an item to the request's `HttpContext` with those scopes when the request is unauthorized, which will be used by `HttpBearerChallengeEvents` to produce the header with the correct value, including the scope parameter.

> This allows the `WWW-Authenticate` value to be dynamic in the sense that different API endpoints may produce different values if they require different scopes.

## Source Code

The sample includes 3 projects:

- `AuthorizationServer` - a Web application running the authorization server, built with Duende Identity Server.
- `RestApi` - a Web application running the REST API.
- `HttpBearerChallenge` - the library used by the `RestApi` project to implement the HTTP Bearer Challenge.

It also includes a simple Postman collection that allows invoking the REST API endpoints to test the bearer challenge.

The magic happens in the `RestApi` `Startup` class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    (...)

    services
        .AddAuthentication("Bearer")
        .AddJwtBearer(
            (options) =>
            {
                options.Authority = "https://localhost:5001";
                options.Audience = "rest-api";
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;
                options.RefreshOnIssuerKeyNotFound = true;
                options.SaveToken = true;

                // NOTE:
                // This builds the WWW-Authenticate header

                options.Events = new HttpBearerChallengeEvents();
            });

    services
        .AddAuthorization(
            (options) =>
            {
                options.AddPolicy(
                    "read-cars",
                    (policy) =>
                    {
                        // NOTE:
                        // This makes the scope required by the endpoints
                        // available for the HttpBearerChallengeBuilder

                        policy.RequireScope("read-cars");
                    });
                options.AddPolicy(
                    "read-trucks",
                    (policy) =>
                    {
                        // NOTE:
                        // This makes the scope required by the endpoints
                        // available for the HttpBearerChallengeBuilder

                        policy.RequireScope("read-trucks");
                    });
            });

    (...)
}
```