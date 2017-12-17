using Autofac;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using System;
using System.Linq;

namespace CouchMan
{
    public class CouchMonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Cluster>()
                   .As<ICluster>()
                   .WithParameter(new TypedParameter(typeof(ClientConfiguration), GetClientConfig()))
                   .SingleInstance();

            builder.RegisterType<PasswordAuthenticator>()
                   .As<IAuthenticator>()
                   .WithParameter("username",GetClusterUsername())
                   .WithParameter("password", GetClusterPassword())
                   .InstancePerLifetimeScope();

            builder.RegisterType<ClusterService>()
                   .As<IClusterService>()
                   .InstancePerLifetimeScope();
        }

        private ClientConfiguration GetClientConfig()
        {
            var urls = new[] { "http://localhost/8091" };

            return new ClientConfiguration
            {
                Servers = urls.Select(url => new Uri(url)).ToList()
            };
        }

        private string GetClusterUsername()
        {
            return "Administrator";
        }

        private string GetClusterPassword()
        {
            return "badpassword";
        }
    }
}
