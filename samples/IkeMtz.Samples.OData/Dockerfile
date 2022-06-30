#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["samples/IkeMtz.Samples.OData/IkeMtz.Samples.OData.csproj", "samples/IkeMtz.Samples.OData/"]
COPY ["src/IkeMtz.NRSRx.Core.OData/IkeMtz.NRSRx.Core.OData.csproj", "src/IkeMtz.NRSRx.Core.OData/"]
COPY ["src/IkeMtz.NRSRx.Core.Web/IkeMtz.NRSRx.Core.Web.csproj", "src/IkeMtz.NRSRx.Core.Web/"]
COPY ["src/IkeMtz.NRSRx.Core.Models/IkeMtz.NRSRx.Core.Models.csproj", "src/IkeMtz.NRSRx.Core.Models/"]
COPY ["samples/IkeMtz.Samples.Models/IkeMtz.Samples.Models.csproj", "samples/IkeMtz.Samples.Models/"]
COPY ["src/Logging/IkeMtz.NRSRx.Logging.ApplicationInsights/IkeMtz.NRSRx.Logging.ApplicationInsights.csproj", "src/Logging/IkeMtz.NRSRx.Logging.ApplicationInsights/"]
RUN dotnet restore "samples/IkeMtz.Samples.OData/IkeMtz.Samples.OData.csproj"
COPY . .
WORKDIR "/src/samples/IkeMtz.Samples.OData"
RUN dotnet build "IkeMtz.Samples.OData.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IkeMtz.Samples.OData.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IkeMtz.Samples.OData.dll"]