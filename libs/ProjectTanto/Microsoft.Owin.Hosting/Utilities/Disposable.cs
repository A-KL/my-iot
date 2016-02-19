using System;

namespace Microsoft.Owin.Hosting.Utilities
{
    internal sealed class Disposable : IDisposable
    {
        private readonly Action dispose;

        public Disposable(Action dispose)
        {
            this.dispose = dispose;
        }

        public void Dispose()
        {
            dispose.Invoke();
        }
    }
}
