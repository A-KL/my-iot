using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Owin.Hosting.Utilities
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    internal class Encapsulate
    {
        private readonly AppFunc _app;
        private readonly IList<KeyValuePair<string, object>> _environmentData;

        public Encapsulate(AppFunc app, IList<KeyValuePair<string, object>> environmentData)
        {
            _app = app;
            _environmentData = environmentData;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            for (var i = 0; i < _environmentData.Count; i++)
            {
                var pair = _environmentData[i];
                object obj;
                if (!environment.TryGetValue(pair.Key, out obj) || obj == null)
                {
                    environment[pair.Key] = pair.Value;
                }
            }

            return _app.Invoke(environment);
        }
    }
}
