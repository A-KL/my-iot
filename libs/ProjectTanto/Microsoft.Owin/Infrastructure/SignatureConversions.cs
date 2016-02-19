﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin.Extensions;
using Owin;

namespace Microsoft.Owin.Infrastructure
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Adds adapters between <typeref name="Func&lt;IDictionary&lt;string,object&gt;, Task&gt;"/> and OwinMiddleware.
    /// </summary>
    public static class SignatureConversions
    {
        /// <summary>
        /// Adds adapters between <typeref name="Func&lt;IDictionary&lt;string,object&gt;, Task&gt;"/> and OwinMiddleware.
        /// </summary>
        /// <param name="app"></param>
        public static void AddConversions(IAppBuilder app)
        {
            app.AddSignatureConversion<AppFunc, OwinMiddleware>(Conversion1);
            app.AddSignatureConversion<OwinMiddleware, AppFunc>(Conversion2);
        }

        private static OwinMiddleware Conversion1(AppFunc next)
        {
            return new AppFuncTransition(next);
        }

        private static AppFunc Conversion2(OwinMiddleware next)
        {
            return new OwinMiddlewareTransition(next).Invoke;
        }
    }
}
