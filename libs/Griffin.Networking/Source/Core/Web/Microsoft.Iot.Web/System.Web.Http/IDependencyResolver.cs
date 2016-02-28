using System.Collections.Generic;

namespace System.Web.Http
{
    public interface IDependencyScope : IDisposable
    {

        object GetService(Type serviceType);
        IEnumerable<object> GetServices(Type serviceType);
    }

    public interface IDependencyResolver : IDependencyScope
    {
        IDependencyScope BeginScope();
    }
}
