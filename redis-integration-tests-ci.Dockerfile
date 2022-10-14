FROM ikemtz/redis-dotnet

COPY . /integrationTests
RUN redis-server && \
  dotnet test /integrationTests/tests/IkeMtz.NRSRx.Events.Redis.Tests --logger trx --configuration debug --collect "XPlat Code Coverage" && \
  dotnet test /integrationTests/tests/IkeMtz.NRSRx.Events.Publishers.Redis.Tests --logger trx --configuration debug --collect "XPlat Code Coverage"

CMD bash