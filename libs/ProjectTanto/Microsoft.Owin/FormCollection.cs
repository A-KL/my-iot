namespace Microsoft.Owin
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the parsed form values.
    /// </summary>
    public class FormCollection : ReadableStringCollection, IFormCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microsoft.Owin.FormCollection" /> class.
        /// </summary>
        /// <param name="store">The store for the form.</param>
        public FormCollection(IDictionary<string, string[]> store)
            : base(store)
        {
        }
    }
}
