FROM ikemtz/redis-dotnet

COPY . /integrationTests
RUN redis-server & \
  dotnet test /integrationTests/tests/IkeMtz.NRSRx.Events.Redis.Tests --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results && \
  dotnet test /integrationTests/tests/IkeMtz.NRSRx.Events.Publishers.Redis.Tests --logger trx --configuration Debug --collect "XPlat Code Coverage" --results-directory /test-results

CMD bash