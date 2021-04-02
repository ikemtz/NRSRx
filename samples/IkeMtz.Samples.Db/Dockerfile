FROM ikemtz/sql_dacpac:latest as sql-temp
ENV SA_PASSWORD=SqlDockerRocks123! \
    ACCEPT_EULA=Y 
 
COPY /bin/Debug/*.dacpac /dacpac/
RUN /opt/mssql/bin/sqlservr & sleep 20 \ 
    && sqlpackage /Action:Publish /TargetServerName:localhost /TargetUser:SA /TargetPassword:$SA_PASSWORD /SourceFile:/dacpac/IkeMtz.Samples.Db.dacpac /TargetDatabaseName:SamplesDb /p:BlockOnPossibleDataLoss=false \ 
    && sleep 20 \ 
    && pkill sqlservr && sleep 10 \ 
    && sudo rm -rf /dacpac 
 
FROM mcr.microsoft.com/mssql/server 
LABEL author="@IkeMtz" 
ENV SA_PASSWORD=SqlDockerRocks123! \ 
    ACCEPT_EULA=Y 
EXPOSE 1433 
 
COPY --from=sql-temp /var/opt/mssql/data/*.ldf /var/opt/mssql/data/ 
COPY --from=sql-temp /var/opt/mssql/data/*.mdf /var/opt/mssql/data/ 
