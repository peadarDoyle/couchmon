using Autofac;
using Couchmon.Couchbase;

namespace Couchmon
{
    public static class CouchmonContext
    {
        private static IContainer _container;

        public static void Initialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CouchMonModule>();
            _container = builder.Build();
        }

        public static T GetInstance<T>() where T : IClusterService
        {
            if (_container == null)
            {
                throw new NotInitializedException();
            }

            using (var scope = _container.BeginLifetimeScope())
            {
                return scope.Resolve<T>();
            }
        }
    }
}
