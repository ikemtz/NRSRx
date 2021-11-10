FROM ikemtz/sql_dacpac:latest
ENV SA_PASSWORD=SqlDockerRocks123! \ 
    ACCEPT_EULA=Y 

COPY --from=build /samples/IkeMtz.Samples.OData.Tests/bin/Debug/net5.0/publish /integrationTests
COPY samples/IkeMtz.Samples.Db/bin/Debug/*.dacpac /dacpac/
COPY samples/IkeMtz.Samples.OData.Tests/bin/Debug/net5.0/publish  /integrationTests
RUN /opt/mssql/bin/sqlservr & sleep 30 \ 
    && sqlpackage /Action:Publish /TargetServerName:localhost /TargetUser:SA /TargetPassword:$SA_PASSWORD /SourceFile:/dacpac/IkeMtz.Samples.Db.dacpac /TargetDatabaseName:SamplesDb /p:BlockOnPossibleDataLoss=false \ 
    && sleep 20 \ 
    && dotnet test IkeMtz.Samples.OData.Tests.dll \
    && pkill sqlservr && sleep 10