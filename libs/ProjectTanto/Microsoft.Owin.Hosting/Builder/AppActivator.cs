// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using Microsoft.Owin.Hosting.Services;

namespace Microsoft.Owin.Hosting.Builder
{
    /// <summary>
    /// Used to instantiate the application entry point. e.g. the Startup class.
    /// </summary>
    public class AppActivator : IAppActivator
    {
        private readonly IServiceProvider services;

        /// <summary>
        /// Creates a new AppActivator.
        /// </summary>
        /// <param name="services"></param>
        public AppActivator(IServiceProvider services)
        {
            this.services = services;
        }

        /// <summary>
        /// Instantiate an instance of the given type, injecting any available services.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual object Activate(Type type)
        {
            return ActivatorUtilities.GetServiceOrCreateInstance(services, type);
        }
    }
}
