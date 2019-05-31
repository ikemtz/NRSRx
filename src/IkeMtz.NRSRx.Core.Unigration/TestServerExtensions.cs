using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration
{
    public static class TestServerExtensions
    {
        public static Context GetDbContext<Context>(this TestServer testServer) where Context : DbContext
        {
            var services = testServer.Host.Services;
            var dbContext = services.GetRequiredService<Context>();
            return dbContext;
        }

        public static ServiceType GetTestService<ServiceType>(this TestServer testServer) where ServiceType : class
        {
            return testServer.Host.Services.GetService<ServiceType>();
        }

        public static ImplementationType GetTestService<ServiceType, ImplementationType>(this TestServer testServer)
            where ServiceType : class
            where ImplementationType : class
        {
            return testServer.GetTestService<ServiceType>() as ImplementationType;
        }
    }
}
