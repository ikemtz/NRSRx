FROM ikemtz/sql_dacpac:latest
ENV SA_PASSWORD=SqlDockerRocks123! \
    ACCEPT_EULA=Y
ENV ASPNETCORE_ENVIRONMENT=development
ENV DbConnectionString="Server=localhost;Database=SamplesDb;User ID=sa;Password=SqlDockerRocks123!;"
COPY samples/IkeMtz.Samples.Db/bin/Debug/*.dacpac /dacpac/
COPY . /src
USER root

RUN /opt/mssql/bin/sqlservr & sleep 30 \
    && sqlpackage /Action:Publish /TargetServerName:localhost /TargetUser:SA /TargetPassword:$SA_PASSWORD /SourceFile:/dacpac/IkeMtz.Samples.Db.dacpac /TargetDatabaseName:SamplesDb /p:BlockOnPossibleDataLoss=false \
    && sleep 20 \
    && dotnet test /src/samples/IkeMtz.Samples.OData.Tests --filter TestCategory=SqlIntegration \
    && dotnet test /src/samples/IkeMtz.Samples.WebApi.Tests --filter TestCategory=SqlIntegration \
    && pkill sqlservr

ENTRYPOINT bash