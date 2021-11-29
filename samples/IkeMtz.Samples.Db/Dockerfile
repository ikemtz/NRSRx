FROM ikemtz/sql_dacpac:latest as sql-temp
ENV SA_PASSWORD=SqlDockerRocks123! \
    ACCEPT_EULA=Y 
 
COPY /bin/Debug/*.dacpac /dacpac/
RUN /opt/mssql/bin/sqlservr & sleep 20 \ 
    && sqlpackage /Action:Publish /TargetServerName:localhost /TargetUser:sa /TargetPassword:$SA_PASSWORD /SourceFile:/dacpac/IkeMtz.Samples.Db.dacpac /TargetDatabaseName:SamplesDb /p:BlockOnPossibleDataLoss=false \ 
    && sleep 20 \ 
    && pkill sqlservr && sleep 10
 
FROM mcr.microsoft.com/mssql/server 
LABEL author="@IkeMtz" 
ENV SA_PASSWORD=SqlDockerRocks123! \ 
    ACCEPT_EULA=Y 
EXPOSE 1433 
 
COPY --from=sql-temp /var/opt/mssql/data/SamplesDb*.ldf /var/opt/mssql/data/ 
COPY --from=sql-temp /var/opt/mssql/data/SamplesDb*.mdf /var/opt/mssql/data/ 

USER root
RUN chown -R mssql /var/opt/mssql/data
USER mssql
