FROM ikemtz/sql_dacpac:latest as sql-temp
ENV SA_PASSWORD=SqlDockerRocks123! \
    ACCEPT_EULA=Y 
 
COPY /bin/Debug/*.dacpac /dacpac/
#Need ample time to allow SQL server to start (30 sec)
RUN /opt/mssql/bin/sqlservr & sleep 30 \ 
    && sqlpackage /Action:Publish /TargetServerName:localhost /TargetUser:sa /TargetPassword:$SA_PASSWORD /SourceFile:/dacpac/IkeMtz.Samples.Db.dacpac /TargetDatabaseName:SamplesDb /p:BlockOnPossibleDataLoss=false \ 
    && sleep 20 \ 
    && pkill sqlservr && sleep 10
 
FROM mcr.microsoft.com/mssql/server 
LABEL author="@IkeMtz" 
ENV SA_PASSWORD=SqlDockerRocks123! \ 
    ACCEPT_EULA=Y
ENV NEW_DB_NAME=SamplesDb
EXPOSE 1433 
 
COPY --from=sql-temp /var/opt/mssql/data/${NEW_DB_NAME}*.ldf /var/opt/mssql/data/
COPY --from=sql-temp /var/opt/mssql/data/${NEW_DB_NAME}*.mdf /var/opt/mssql/data/
ENV PATH="/opt/mssql-tools/bin:${PATH}"
USER root
RUN chown -R mssql /var/opt/mssql/data && \
    chmod +x /opt/mssql-tools/bin/sqlcmd
USER mssql
