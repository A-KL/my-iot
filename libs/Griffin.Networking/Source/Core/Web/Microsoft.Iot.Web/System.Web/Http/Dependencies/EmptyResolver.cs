using System.Collections.Generic;
using System.Linq;

namespace System.Web.Http.Dependencies
{
    internal class EmptyResolver : IDependencyResolver, IDependencyScope, IDisposable
    {
        private static readonly IDependencyResolver _instance = (IDependencyResolver)new EmptyResolver();

        public static IDependencyResolver Instance
        {
            get
            {
                return EmptyResolver._instance;
            }
        }

        private EmptyResolver()
        {
        }

        public IDependencyScope BeginScope()
        {
            return (IDependencyScope)this;
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return (object)null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Enumerable.Empty<object>();
        }
    }
}
