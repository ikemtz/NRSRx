FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
COPY . .
ENTRYPOINT ["dotnet", "IkeMtz.Samples.OData.dll"]
HEALTHCHECK CMD curl --fail http://localhost:5000/healthz || exit
