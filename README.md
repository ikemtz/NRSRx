[![Build Status](https://ikemtz.visualstudio.com/Devops/_apis/build/status/NRSRx%20Packages/ikemtz.NRSRx?branchName=master)](https://ikemtz.visualstudio.com/Devops/_build/latest?definitionId=32&branchName=master)
[![Nuget Package](https://img.shields.io/nuget/v/IkeMtz.NRSRx.Core.Models.svg)](https://www.nuget.org/packages?q=nrsrx) [![](https://img.shields.io/nuget/dt/IkeMtz.NRSRx.Core.Models)](https://www.nuget.org/packages/IkeMtz.NRSRx.Core.Models/)
[![Release Status](https://ikemtz.vsrm.visualstudio.com/_apis/public/Release/badge/9abb8a0b-71e1-4090-b59c-46edc077875f/8/8)](https://ikemtz.visualstudio.com/Devops/_release?definitionId=8&view=mine&_a=releases)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=coverage)](https://sonarcloud.io/dashboard?id=NRSRx) 
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=security_rating)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=NRSRx&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=NRSRx)
[![Nuget Package](https://img.shields.io/nuget/dt/IkeMtz.NRSRx.Core.Models.svg)](https://www.nuget.org/packages?q=nrsrx)

# NRSRX
NRSRx is an opinionated, flexible, and extensible framework that will expedite the development of back-end services built on asp.net 6 - 8.  It does this by addressing some cross-cutting concerns that typically require development effort, letting you focus on your business challenges.

## Cross cutting concerns

NRSRx is highly configurable and extensible, but these are the cross-cutting concerns that are addressed:

*  Externalized configuration 
    -	appsettings.json
    -	User Secrets (useful in development environments)
    - Environment Variables (useful in containerized and cloud platforms)
*  Authentication
    -	JWT based authentication (commonly used in [OAuth2](https://oauth.net/2/) and [OIDC](https://openid.net/connect/) authentication workflows)
* Authorization
    -	Role-based authorization via asp.net request authorization filter
*  Swagger
    -	Setup as an OIDC client to facilitate authorization
*  Data persistence
    -	Entity Framework with entity-based auditing
*  Multi-Tenancy
    -	Asp.net request authorization filter
*  Versioning
   *  Api and model 
   *  Adhere's to [Microsoft REST Guidelines for versioning](https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#12-versioning)
*  Logging
    -	Application Insights (supported via [IkeMtz.NRSRx.Logging.ApplicationInsights](https://www.nuget.org/packages/IkeMtz.NRSRx.Logging.ApplicationInsights/) library)
    -	Elastisearch (supported via [IkeMtz.NRSRx.Logging.Elasticsearch](https://www.nuget.org/packages/IkeMtz.NRSRx.Logging.Elasticsearch/) library)
    -	Splunk (Support via [IkeMtz.NRSRx.Logging.Splunk](https://www.nuget.org/packages/IkeMtz.NRSRx.Logging.Splunk/) library)
* Eventing
    -	Azure Service Bus (supported via [IkeMtz.NRSRx.Events.Publishers.ServiceBus](https://www.nuget.org/packages/IkeMtz.NRSRx.Events.Publishers.ServiceBus/) library)
    -	Redis Streams (supported via [IkeMtz.NRSRx.Events.Publishers.Redis](https://www.nuget.org/packages/IkeMtz.NRSRx.Events.Publishers.Redis/) library)
* Unit testability
    - MSTest (services built on NRSRx can easily achieve 95%+ code coverage)

## Adaptability

Most of the features built into NRSRx are customizable or removable all together.  Don't want authentication?  Remove it.  Nothing in the NRSRx framework is private, internal or sealed, so expansion is a breeze.

## Flavors

NRSRx services come in three different flavors.

- OData
- WebApi
- SignalR
- GraphQL (coming soon)

## Sample Services

- [Samples.Events.Redis](./samples/IkeMtz.Samples.Events.Redis): 
  
  A sample WebApi web application that will publish events to Redis streams and persist data to SQL server using Entity Framework.

- [Samples.OData](./samples/IkeMtz.Samples.OData)

  A sample OData web application that reads data from SQL Server using Entity Framework.

- [Samples.SignalR](./samples/IkeMtz.Samples.SignalR)

  A sample SignalR web application.

## Quality

Every change that goes into the NRSRx framework goes through full unit and integration tests on [Azure Devops](https://ikemtz.visualstudio.com/Devops/_build?definitionId=32) pipelines.
