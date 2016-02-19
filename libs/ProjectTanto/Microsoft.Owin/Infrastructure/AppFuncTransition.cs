using System.Threading.Tasks;

namespace Microsoft.Owin.Infrastructure
{
    using System;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, Task>;

    /// <summary>
    /// Converts between an OwinMiddlware and an <typeref name="Func&lt;IDictionary&lt;string,object&gt;, Task&gt;"/>.
    /// </summary>
    internal sealed class AppFuncTransition : OwinMiddleware
    {
        private readonly AppFunc _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public AppFuncTransition(AppFunc next) : base(null)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task Invoke(IOwinContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return _next(context.Environment);
        }
    }
}
