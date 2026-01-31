# IkeMtz.NRSRx.Core.WebApi

[![Nuget Package](https://img.shields.io/nuget/v/IkeMtz.NRSRx.Core.WebApi.svg)](https://www.nuget.org/packages/IkeMtz.NRSRx.Core.WebApi/)

## Overview

`IkeMtz.NRSRx.Core.WebApi` is a foundational library for building RESTful Web API microservices with the NRSRx framework. NRSRx is an opinionated framework that configures common cross-cutting concerns, allowing developers to focus on business logic rather than infrastructure setup.

## Features

- **JWT Authentication & Authorization**: Pre-configured OAuth2/OIDC authentication with role-based authorization
- **API Versioning**: URL segment-based versioning following [Microsoft REST Guidelines](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#12-versioning)
- **Swagger/OpenAPI Integration**: Auto-generated API documentation with OAuth2 client support
- **Health Checks**: Built-in endpoint (`/healthz`) with extensible health check configuration
- **JSON & XML Support**: Newtonsoft.Json with camel casing, null handling, and XML serialization
- **Configuration Management**: Support for appsettings.json, user secrets, and environment variables
- **Logging Integration**: Extensible logging setup (supports Application Insights, Elasticsearch, Splunk)
- **Event Publishing**: Extensible publisher setup for Azure Service Bus and Redis Streams
- **Entity Framework Support**: Database context configuration with connection string management

## Key Components

### CoreWebApiStartup

The abstract base class that provides the core Web API setup. Key methods to override:

#### Required Overrides

- `ServiceTitle`: Display name for your microservice
- `StartupAssembly`: Assembly containing your controllers

#### Optional Overrides

- `SetupDatabase()`: Configure Entity Framework DbContext
- `SetupHealthChecks()`: Add custom health checks
- `SetupPublishers()`: Configure event publishers (Service Bus, Redis)
- `SetupMiscDependencies()`: Register additional services
- `SetupLogging()`: Configure logging providers
- `SetupAuthentication()`: Customize JWT authentication
- `SetupSwagger()`: Customize Swagger/OpenAPI configuration

### Built-in Configuration

#### API Versioning

- URL segment-based: `/v1/products`, `/v2/products`
- Headers: `api-supported-versions` and `api-deprecated-versions`
- Format: `v{major}.{minor}`

#### JSON Serialization

- Camel case property names
- Null value ignoring
- Enum string conversion
- Reference loop handling

#### Authentication

- JWT Bearer token authentication
- Configurable claim mappings (name, role)
- Multi-audience support

#### Swagger UI

- OAuth2 Authorization Code Flow with PKCE
- Auto-discovery from OIDC provider
- XML documentation comments
- Versioned endpoint organization

## Endpoints

### Health Check

- **URL**: `/healthz`
- **Method**: GET
- **Response**: 200 OK (Healthy) or 503 Service Unavailable (Unhealthy)

### API Endpoints

All controllers inherit API versioning and are automatically registered with Swagger documentation.

## Related Packages

- [IkeMtz.NRSRx.Core.Models](https://www.nuget.org/packages/IkeMtz.NRSRx.Core.Models/): Core model definitions
- [IkeMtz.NRSRx.Core.Web](https://www.nuget.org/packages/IkeMtz.NRSRx.Core.Web/): Shared web infrastructure
- [IkeMtz.NRSRx.Core.EntityFramework](https://www.nuget.org/packages/IkeMtz.NRSRx.Core.EntityFramework/): Entity Framework helpers
- [IkeMtz.NRSRx.Events.Publishers.ServiceBus](https://www.nuget.org/packages/IkeMtz.NRSRx.Events.Publishers.ServiceBus/): Azure Service Bus event publishing
- [IkeMtz.NRSRx.Events.Publishers.Redis](https://www.nuget.org/packages/IkeMtz.NRSRx.Events.Publishers.Redis/): Redis Streams event publishing

## Samples

See the [sample WebApi project](https://github.com/ikemtz/NRSRx/tree/master/samples/IkeMtz.Samples.WebApi) for a complete working example.

## Documentation

For complete framework documentation, visit the [NRSRx GitHub repository](https://github.com/ikemtz/NRSRx).

## License

MIT © [IkeMtz](https://github.com/ikemtz)
