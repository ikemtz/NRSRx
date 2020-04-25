using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class TestServerExtensions
  {
    public static TContext GetDbContext<TContext>(this TestServer testServer) where TContext : DbContext
    {
      return GetTestService<TContext>(testServer);
    }

    public static TServiceType GetTestService<TServiceType>(this TestServer testServer) where TServiceType : class
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      return testServer.Host.Services.GetService<TServiceType>();
    }

    public static TImplementationType GetTestService<TServiceType, TImplementationType>(this TestServer testServer)
        where TServiceType : class
        where TImplementationType : class
    {
      return testServer.GetTestService<TServiceType>() as TImplementationType;
    }
  }
}
