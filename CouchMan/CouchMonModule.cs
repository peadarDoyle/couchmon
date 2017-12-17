using Autofac;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using System;
using System.Configuration;
using System.Linq;

namespace CouchMan
{
    public class CouchMonModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Cluster>()
                   .As<ICluster>()
                   .WithParameter(new TypedParameter(typeof(ClientConfiguration), GetCouchbaseClientConfig()))
                   .SingleInstance();

            builder.RegisterType<PasswordAuthenticator>()
                   .As<IAuthenticator>()
                   .WithParameter("username",GetCouchbaseUsername())
                   .WithParameter("password", GetCouchbasePassword())
                   .InstancePerLifetimeScope();

            builder.RegisterType<ClusterService>()
                   .As<IClusterService>()
                   .InstancePerLifetimeScope();
        }

        private ClientConfiguration GetCouchbaseClientConfig()
        {
            string ipCsv = ConfigurationManager.AppSettings["couchbase.ip.csv"];
            string transport = ConfigurationManager.AppSettings["couchbase.transport"];

            string[] ips = ipCsv.Split(';')
                                .Distinct()
                                .Select(ip => $"{transport}://{ip}")
                                .ToArray();

            return new ClientConfiguration
            {
                Servers = ips.Select(ip => new Uri(ip)).ToList()
            };
        }

        private string GetCouchbaseUsername()
        {
            return ConfigurationManager.AppSettings["couchbase.username"];
        }

        private string GetCouchbasePassword()
        {
            return ConfigurationManager.AppSettings["couchbase.password"];
        }
    }
}
