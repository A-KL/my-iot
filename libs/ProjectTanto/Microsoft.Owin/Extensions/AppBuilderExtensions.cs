using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;

namespace Microsoft.Owin.Extensions
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    /// <summary>
    /// Extension methods for IAppBuilder.
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// The Build is called at the point when all of the middleware should be chained
        /// together. May be called to build pipeline branches.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The request processing entry point for this section of the pipeline.</returns>
        public static AppFunc Build(this IAppBuilder builder)
        {
            return builder.Build<AppFunc>();
        }

        /// <summary>
        /// The Build is called at the point when all of the middleware should be chained
        /// together. May be called to build pipeline branches.
        /// </summary>
        /// <typeparam name="TApp">The application signature.</typeparam>
        /// <param name="builder"></param>
        /// <returns>The request processing entry point for this section of the pipeline.</returns>
        public static TApp Build<TApp>(this IAppBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            return (TApp)builder.Build(typeof(TApp));
        }

        /// <summary>
        /// Adds converters for adapting between disparate application signatures.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="conversion"></param>
        public static void AddSignatureConversion(this IAppBuilder builder, Delegate conversion)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            object obj;
            if (builder.Properties.TryGetValue("builder.AddSignatureConversion", out obj))
            {
                var action = obj as Action<Delegate>;
                if (action != null)
                {
                    action(conversion);
                    return;
                }
            }
            throw new MissingMethodException(builder.GetType().FullName, new Exception("AddSignatureConversion"));
        }

        /// <summary>
        /// Adds converters for adapting between disparate application signatures.
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="builder"></param>
        /// <param name="conversion"></param>
        public static void AddSignatureConversion<T1, T2>(this IAppBuilder builder, Func<T1, T2> conversion)
        {
            AddSignatureConversion(builder, (Delegate)conversion);
        }
    }
}
