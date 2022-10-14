FROM ikemtz/redis-dotnet

COPY . /integrationTests
RUN redis-server & sleep 15 && \
  dotnet test /integrationTests/tests/IkeMtz.NRSRx.Events.Redis.Tests --filter TestCategory=RedisIntegration