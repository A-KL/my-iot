namespace Microsoft.Owin.Hosting.Starter
{
    using System;

    /// <summary>
    /// This attribute is used to identify custom hosting starters that may be loaded at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class HostingStarterAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingStarterType"></param>
        public HostingStarterAttribute(Type hostingStarterType)
        {
            HostingStarterType = hostingStarterType;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type HostingStarterType { get; private set; }
    }
}
