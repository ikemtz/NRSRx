FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
COPY ./build ./build
COPY ./src ./src
COPY ./samples/IkeMtz.Samples.Models ./samples/IkeMtz.Samples.Models
COPY ./samples/IkeMtz.Samples.WebApi ./samples/IkeMtz.Samples.WebApi
RUN dotnet publish ./samples/IkeMtz.Samples.WebApi --output ./dist --no-self-contained --runtime linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
COPY --from=publish /dist .
ENTRYPOINT ["dotnet", "IkeMtz.Samples.WebApi.dll"]