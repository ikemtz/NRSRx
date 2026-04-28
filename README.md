[![Build Status](https://ikemtz.visualstudio.com/Devops/_apis/build/status/NRSRx%20Packages/ikemtz.NRSRx?branchName=master)](https://ikemtz.visualstudio.com/Devops/_build/latest?definitionId=32&branchName=master)
[![Nuget Package](https://img.shields.io/nuget/v/IkeMtz.NRSRx.Core.Models.svg)](https://www.nuget.org/packages?q=nrsrx) [![](https://img.shields.io/nuget/dt/IkeMtz.NRSRx.Core.Models)](https://www.nuget.org/packages/IkeMtz.NRSRx.Core.Models/)
[![Release Status](https://ikemtz.vsrm.visualstudio.com/_apis/public/Release/badge/9abb8a0b-71e1-4090-b59c-46edc077875f/8/8)](https://ikemtz.visualstudio.com/Devops/_release?definitionId=8&view=mine&_a=releases)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=coverage)](https://sonarcloud.io/dashboard?id=NRSRx) 
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=security_rating)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Nuget Package](https://img.shields.io/nuget/dt/IkeMtz.NRSRx.Core.Models.svg)](https://www.nuget.org/packages?q=nrsrx)

# NRSRx
NRSRx is an opinionated, extensible framework that accelerates building back-end services on modern .NET. The repository contains reusable libraries, samples and utilities used across microservices built by the authoring organization.

Supported framework targets in this workspace:

- `.NET 9` (apps and samples)
- `.NET Standard 2.1` and `.NET Standard 2.0` (libraries)

Key features and cross-cutting concerns:

- Externalized configuration (`appsettings.json`, user secrets, environment variables)
- Authentication and authorization (JWT / OIDC integration)
- Swagger / OpenAPI support (Swashbuckle + `Microsoft.OpenApi` models)
- OData support (filters/operation filters for documentation)
- Data persistence (Entity Framework Core patterns)
- Logging (Application Insights, Elasticsearch, Splunk extensions)
- Eventing (Azure Service Bus, Redis Streams)
- API Versioning and model version support

Projects and samples

- Core libraries: `src/IkeMtz.NRSRx.Core.*`
- Samples: `samples/IkeMtz.Samples.OData`, `samples/IkeMtz.Samples.WebApi`, `samples/IkeMtz.Samples.SignalR`, `samples/IkeMtz.Samples.Events.Redis`

Getting started

1. Install the .NET SDK for the targeted runtimes (recommended: .NET 9).
2. Restore and build:

```bash
dotnet restore
dotnet build -c Release
```

3. Run tests:

```bash
dotnet test
```

Running samples

- Each sample is a self-contained ASP.NET app under `samples/`.
- From the sample folder, run `dotnet run` or use your IDE.
- Swagger UI is available when running a sample if `DisableSwagger` is not set; several projects include document/operation filters for OData and reverse-proxy scenarios.

Swagger and OpenAPI notes

- This solution uses `Swashbuckle.AspNetCore` and `Microsoft.OpenApi.Models`. Document and operation filters have been migrated to consume `Microsoft.OpenApi` model types (e.g. `OpenApiDocument`, `OpenApiOperation`, `OpenApiSchema`).

Contributing

- Fork and submit pull requests.
- Follow existing coding patterns and add unit tests for new behavior.

License and contact

- See the repository root for license information. For questions, open an issue or PR.

