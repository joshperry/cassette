using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using Cassette.Configuration;
using Cassette.Web;

namespace Cassette
{
    class TestableWebHost : WebHost
    {
        readonly string sourceDirectory;
        readonly RouteCollection routes;
        readonly Func<HttpContextBase> getHttpContext;
        readonly bool isAspNetDebuggingEnabled;
        readonly List<IConfiguration<BundleCollection>> bundleConfigurations = new List<IConfiguration<BundleCollection>>();

        public TestableWebHost(string sourceDirectory, RouteCollection routes, Func<HttpContextBase> getHttpContext, bool isAspNetDebuggingEnabled = false)
        {
            this.sourceDirectory = Path.GetFullPath(sourceDirectory);
            this.routes = routes;
            this.getHttpContext = getHttpContext;
            this.isAspNetDebuggingEnabled = isAspNetDebuggingEnabled;
        }

        protected override IEnumerable<Assembly> LoadAssemblies()
        {
            var dlls = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            return dlls.Select(Assembly.LoadFrom);
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.Register(typeof(IEnumerable<IConfiguration<BundleCollection>>), bundleConfigurations);
        }

        public void AddBundleConfiguration(IConfiguration<BundleCollection> bundleConfiguration)
        {
            bundleConfigurations.Add(bundleConfiguration);
        }

        protected override HttpContextBase HttpContext()
        {
            return getHttpContext();
        }

        protected override string AppDomainAppPath
        {
            get { return sourceDirectory; }
        }

        protected override string AppDomainAppVirtualPath
        {
            get { return "/"; }
        }

        protected override RouteCollection Routes
        {
            get { return routes; }
        }

        protected override CassetteConfigurationSection GetConfigurationSection()
        {
            return new CassetteConfigurationSection
            {
                RewriteHtml = false
            };
        }

        protected override bool IsAspNetDebuggingEnabled
        {
            get { return isAspNetDebuggingEnabled; }
        }
    }
}