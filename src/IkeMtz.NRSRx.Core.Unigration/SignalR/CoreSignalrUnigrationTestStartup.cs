using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using IkeMtz.NRSRx.Core.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.SignalR
{
  public abstract class CoreSignalrUnigrationTestStartup<TStartup> : CoreSignalrStartup
        where TStartup : CoreSignalrStartup
  {
    public TStartup Startup { get; private set; }
    protected CoreSignalrUnigrationTestStartup(TStartup startup) : base(startup?.Configuration)
    {
      this.Startup = startup;
    }

    public override string MicroServiceTitle => Startup.MicroServiceTitle;

    public override Assembly StartupAssembly => Startup.StartupAssembly;

    protected TestContext TestContext { get; private set; }

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      TestContext = app.ApplicationServices.GetService<TestContext>();
      _ = app.UseTestContextRequestLogger(TestContext);
      base.Configure(app, env);
    }
  }
}
