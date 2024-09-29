using System;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Provides extension methods for the <see cref="TestServer"/> class to retrieve services and DbContext instances.
  /// </summary>
  public static class TestServerExtensions
  {
    /// <summary>
    /// Retrieves the specified <see cref="DbContext"/> from the test server's service provider.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext.</typeparam>
    /// <param name="testServer">The test server instance.</param>
    /// <returns>The DbContext instance.</returns>
    public static TContext GetDbContext<TContext>(this TestServer testServer) where TContext : DbContext
    {
      return GetTestService<TContext>(testServer);
    }

    /// <summary>
    /// Retrieves the specified service from the test server's service provider.
    /// </summary>
    /// <typeparam name="TServiceType">The type of the service.</typeparam>
    /// <param name="testServer">The test server instance.</param>
    /// <returns>The service instance.</returns>
    public static TServiceType GetTestService<TServiceType>(this TestServer testServer) where TServiceType : class
    {
      testServer = testServer ?? throw new ArgumentNullException(nameof(testServer));
      return testServer.Host.Services.GetService<TServiceType>();
    }

    /// <summary>
    /// Retrieves the specified service from the test server's service provider and casts it to the specified implementation type.
    /// </summary>
    /// <typeparam name="TServiceType">The type of the service.</typeparam>
    /// <typeparam name="TImplementationType">The type of the implementation.</typeparam>
    /// <param name="testServer">The test server instance.</param>
    /// <returns>The service instance cast to the specified implementation type.</returns>
    public static TImplementationType GetTestService<TServiceType, TImplementationType>(this TestServer testServer)
        where TServiceType : class
        where TImplementationType : class
    {
      return testServer.GetTestService<TServiceType>() as TImplementationType;
    }
  }
}
