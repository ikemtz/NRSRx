FROM ikemtz/sql_dacpac:latest
ENV SA_PASSWORD=SqlDockerRocks123! \ 
    ACCEPT_EULA=Y 
ENV ASPNETCORE_ENVIRONMENT=development
ENV DbConnectionString="Server=.;Database=SamplesDb;User ID=sa;Password=SqlDockerRocks123!;"
COPY samples/IkeMtz.Samples.Db/bin/Debug/*.dacpac /dacpac/
COPY samples/IkeMtz.Samples.OData.Tests/bin/Debug/net6.0/linux-x64/publish/ /src/samples/IkeMtz.Samples.OData.Tests/bin/Debug/net6.0/linux-x64/publish
COPY samples/IkeMtz.Samples.WebApi.Tests/bin/Debug/net6.0/linux-x64/publish/ /src/samples/IkeMtz.Samples.WebApi.Tests/bin/Debug/net6.0/linux-x64/publish
USER root

#Need ample time to allow SQL server to start (30 sec)
RUN /opt/mssql/bin/sqlservr & sleep 30 \ 
    && sqlpackage /Action:Publish /TargetServerName:localhost /TargetUser:SA /TargetPassword:$SA_PASSWORD /SourceFile:/dacpac/IkeMtz.Samples.Db.dacpac /TargetDatabaseName:SamplesDb /p:BlockOnPossibleDataLoss=false \ 
    && sleep 60 \
    && dotnet test /src/samples/IkeMtz.Samples.OData.Tests/bin/Debug/net6.0/linux-x64/publish/IkeMtz.Samples.OData.Tests.dll --filter TestCategory=SqlIntegration --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results \
    && dotnet test /src/samples/IkeMtz.Samples.WebApi.Tests/bin/Debug/net6.0/linux-x64/publish/IkeMtz.Samples.WebApi.Tests.dll --filter TestCategory=SqlIntegration --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results \
    && pkill sqlservr

ENTRYPOINT bash