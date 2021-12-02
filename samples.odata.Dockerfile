FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
COPY ./build ./build
COPY ./src ./src
COPY ./samples/IkeMtz.Samples.OData ./samples/IkeMtz.Samples.OData
RUN dotnet publish ./samples/IkeMtz.Samples.OData --output ./dist --no-self-contained --runtime linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
COPY --from=publish /dist .
ENTRYPOINT ["dotnet", "IkeMtz.Samples.OData.dll"]