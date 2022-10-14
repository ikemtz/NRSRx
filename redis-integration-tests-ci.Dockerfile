FROM ikemtz/redis-dotnet

COPY . /src
RUN redis-server & \
  cd /src && \
  dotnet test tests/IkeMtz.NRSRx.Events.Redis.Tests --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results && \
  dotnet test tests/IkeMtz.NRSRx.Events.Publishers.Redis.Tests --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results && \
  sleep 10 && \
  find /test-results -type f -name coverage.cobertura.xml -exec sed -i 's->\/src->.-' {} +

CMD bash