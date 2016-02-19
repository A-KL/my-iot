// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Owin;

namespace Microsoft.Owin.Hosting.ServerFactory
{
    /// <summary>
    /// The basic ServerFactory contract.
    /// </summary>
    public class ServerFactoryAdapter : IServerFactoryAdapter
    {
        private readonly IServerFactoryActivator activator;
        private readonly Type serverFactoryType;
        private object serverFactory;

        /// <summary>
        /// Creates a wrapper around the given server factory instance.
        /// </summary>
        /// <param name="serverFactory"></param>
        public ServerFactoryAdapter(object serverFactory)
        {
            if (serverFactory == null)
            {
                throw new ArgumentNullException("serverFactory");
            }

            this.serverFactory = serverFactory;
            serverFactoryType = serverFactory.GetType();
            activator = null;
        }

        /// <summary>
        /// Creates a wrapper around the given server factory type.
        /// </summary>
        /// <param name="serverFactoryType"></param>
        /// <param name="activator"></param>
        public ServerFactoryAdapter(Type serverFactoryType, IServerFactoryActivator activator)
        {
            if (serverFactoryType == null)
            {
                throw new ArgumentNullException("serverFactoryType");
            }
            if (activator == null)
            {
                throw new ArgumentNullException("activator");
            }

            this.serverFactoryType = serverFactoryType;
            this.activator = activator;
        }

        /// <summary>
        /// Calls the optional Initialize method on the server factory.
        /// The method may be static or instance, and may accept either
        /// an IAppBuilder or the IAppBuilder.Properties IDictionary&lt;string, object&gt;.
        /// </summary>
        /// <param name="builder"></param>
        public virtual void Initialize(IAppBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            var initializeMethod = serverFactoryType.GetMethod("Initialize", new[] { typeof(IAppBuilder) });

            if (initializeMethod != null)
            {
                if (!initializeMethod.IsStatic && serverFactory == null)
                {
                    serverFactory = activator.Activate(serverFactoryType);
                }
                initializeMethod.Invoke(serverFactory, new object[] { builder });
                return;
            }

            initializeMethod = serverFactoryType.GetMethod("Initialize", new[] { typeof(IDictionary<string, object>) });

            if (initializeMethod != null)
            {
                if (!initializeMethod.IsStatic && serverFactory == null)
                {
                    serverFactory = activator.Activate(serverFactoryType);
                }
                initializeMethod.Invoke(serverFactory, new object[] { builder.Properties });
                return;
            }
        }

        /// <summary>
        /// Calls the Create method on the server factory.
        /// The method may be static or instance, and may accept the AppFunc and the 
        /// IAppBuilder.Properties IDictionary&lt;string, object&gt;.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual IDisposable Create(IAppBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            // TODO: AmbiguousMatchException is throw if there are multiple Create methods. Loop through them and try each.
            MethodInfo serverFactoryMethod = serverFactoryType.GetMethod("Create");
            if (serverFactoryMethod == null)
            {
                // TODO: More detailed error message.
                throw new MissingMethodException("ServerFactory Create");
            }

            // TODO: IAppBuilder support? Initialize supports it.

            ParameterInfo[] parameters = serverFactoryMethod.GetParameters();
            if (parameters.Length != 2)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.Exception_ServerFactoryParameterCount {0}", serverFactoryType));
            }
            if (parameters[1].ParameterType != typeof(IDictionary<string, object>))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Resources.Exception_ServerFactoryParameterType {0}", serverFactoryType));
            }

            // let's see if we don't have the correct callable type for this server factory
            var app = builder.Build(parameters[0].ParameterType);

            if (!serverFactoryMethod.IsStatic && serverFactory == null)
            {
                serverFactory = activator.Activate(serverFactoryType);
            }
            return (IDisposable)serverFactoryMethod.Invoke(serverFactory, new[] { app, builder.Properties });
        }
    }
}
