FROM ikemtz/sql_dacpac:latest
ENV SA_PASSWORD=SqlDockerRocks123! \ 
    ACCEPT_EULA=Y 
ENV ASPNETCORE_ENVIRONMENT=development
ENV DbConnectionString="Server=.;Database=SamplesDb;User ID=sa;Password=SqlDockerRocks123!;"
COPY samples/IkeMtz.Samples.Db/bin/Debug/*.dacpac /dacpac/
COPY samples/IkeMtz.Samples.OData.Tests/bin/Debug/net6.0/linux-x64/publish/ /integrationTests
USER root

RUN /opt/mssql/bin/sqlservr & sleep 30 \ 
    && sqlpackage /Action:Publish /TargetServerName:localhost /TargetUser:SA /TargetPassword:$SA_PASSWORD /SourceFile:/dacpac/IkeMtz.Samples.Db.dacpac /TargetDatabaseName:SamplesDb /p:BlockOnPossibleDataLoss=false \ 
    && sleep 20 \
    && dotnet test /integrationTests/IkeMtz.Samples.OData.Tests.dll --filter TestCategory=SqlIntegration \
    && pkill sqlservr