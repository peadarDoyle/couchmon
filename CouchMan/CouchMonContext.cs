using Autofac;

namespace CouchMan
{
    public static class CouchMonContext
    {
        private static IContainer _container;

        public static void Initialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<CouchMonModule>();
            _container = builder.Build();
        }

        public static T GetInstance<T>()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                return scope.Resolve<T>();
            }
        }
    }
}
