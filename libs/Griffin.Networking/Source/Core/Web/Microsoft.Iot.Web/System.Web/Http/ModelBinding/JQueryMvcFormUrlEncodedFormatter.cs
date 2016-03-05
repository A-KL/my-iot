// Decompiled with JetBrains decompiler
// Type: System.Web.Http.ModelBinding.JQueryMvcFormUrlEncodedFormatter
// Assembly: System.Web.Http, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: F99F496C-B0D2-49C1-A945-C1FCABCE1695
// Assembly location: E:\Assembla-SVN\3DProjects\MyProjects\Windows\WebStreamingService\WebStreamingService\Bin\System.Web.Http.dll

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace System.Web.Http.ModelBinding
{
    /// <summary>
    /// Represents the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter"/> class for handling HTML form URL-ended data, also known as application/x-www-form-urlencoded.
    /// </summary>
    public class JQueryMvcFormUrlEncodedFormatter : FormUrlEncodedMediaTypeFormatter
    {
        private readonly HttpConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Web.Http.ModelBinding.JQueryMvcFormUrlEncodedFormatter"/> class.
        /// </summary>
        public JQueryMvcFormUrlEncodedFormatter()
        {
        }

        public JQueryMvcFormUrlEncodedFormatter(HttpConfiguration config)
        {
            this._configuration = config;
        }

        /// <summary>
        /// Determines whether this <see cref="T:System.Web.Http.ModelBinding.JQueryMvcFormUrlEncodedFormatter"/> can read objects of the specified <paramref name="type"/>.
        /// </summary>
        /// 
        /// <returns>
        /// true if objects of this type can be read; otherwise false.
        /// </returns>
        /// <param name="type">The type of object that will be read.</param>
        public override bool CanReadType(Type type)
        {
            if (type == (Type)null)
                throw new ArgumentNullException("type");
            return true;
        }

        /// <summary>
        /// Reads an object of the specified <paramref name="type"/> from the specified stream. This method is called during deserialization.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="T:System.Threading.Tasks.Task"/> whose result will be the object instance that has been read.
        /// </returns>
        /// <param name="type">The type of object to read.</param><param name="readStream">The <see cref="T:System.IO.Stream"/> from which to read.</param><param name="content">The content being read.</param><param name="formatterLogger">The <see cref="T:System.Net.Http.Formatting.IFormatterLogger"/> to log events to.</param>
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == (Type)null)
                throw new ArgumentNullException("type");
            if (readStream == null)
                throw new ArgumentNullException("readStream");
            if (base.CanReadType(type))
                return base.ReadFromStreamAsync(type, readStream, content, formatterLogger);
            return this.ReadFromStreamAsyncCore(type, readStream, content, formatterLogger);
        }

        private async Task<object> ReadFromStreamAsyncCore(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            object obj = await base.ReadFromStreamAsync(typeof(FormDataCollection), readStream, content, formatterLogger);
            FormDataCollection fd = (FormDataCollection)obj;
            object obj1;
            try
            {
                obj1 = null;
                    // FormDataCollectionExtensions.ReadAs(fd, type, string.Empty, this.RequiredMemberSelector, formatterLogger, this._configuration);
            }
            catch (Exception ex)
            {
                if (formatterLogger == null)
                {
                    throw;
                }
                else
                {
                    formatterLogger.LogError(string.Empty, ex);
                    obj1 = MediaTypeFormatter.GetDefaultValueForType(type);
                }
            }
            return obj1;
        }
    }
}
