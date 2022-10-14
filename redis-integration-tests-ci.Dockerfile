FROM ikemtz/redis-dotnet

COPY . /src
RUN redis-server & \
  dotnet test /src/tests/IkeMtz.NRSRx.Events.Redis.Tests --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results && \
  dotnet test /src/tests/IkeMtz.NRSRx.Events.Publishers.Redis.Tests --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results

CMD bash