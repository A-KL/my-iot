// Decompiled with JetBrains decompiler
// Type: System.Web.Http.ModelBinding.FormDataCollectionExtensions
// Assembly: System.Web.Http, Version=5.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: F99F496C-B0D2-49C1-A945-C1FCABCE1695
// Assembly location: E:\Assembla-SVN\3DProjects\MyProjects\Windows\WebStreamingService\WebStreamingService\Bin\System.Web.Http.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http.Formatting;
using System.Text;

namespace System.Web.Http.ModelBinding
{
    /// <summary>
    /// Represents the extensions for the collection of form data.
    /// </summary>
    public static class FormDataCollectionExtensions
    {
        internal static string NormalizeJQueryToMvc(string key)
        {
            if (key == null)
                return string.Empty;
            StringBuilder stringBuilder = (StringBuilder)null;
            int startIndex1 = 0;
            do
            {
                int startIndex2 = key.IndexOf('[', startIndex1);
                if (startIndex2 < 0)
                {
                    if (startIndex1 == 0)
                        return key;
                    stringBuilder = stringBuilder ?? new StringBuilder();
                    stringBuilder.Append(key, startIndex1, key.Length - startIndex1);
                    break;
                }
                stringBuilder = stringBuilder ?? new StringBuilder();
                stringBuilder.Append(key, startIndex1, startIndex2 - startIndex1);
                int num = key.IndexOf(']', startIndex2);
                if (num == -1)
                    throw Error.Argument("key", "SRResources.JQuerySyntaxMissingClosingBracket", new object[0]);
                if (num != startIndex2 + 1)
                {
                    if (char.IsDigit(key[startIndex2 + 1]))
                    {
                        stringBuilder.Append(key, startIndex2, num - startIndex2 + 1);
                    }
                    else
                    {
                        stringBuilder.Append('.');
                        stringBuilder.Append(key, startIndex2 + 1, num - startIndex2 - 1);
                    }
                }
                startIndex1 = num + 1;
            }
            while (startIndex1 < key.Length);
            return stringBuilder.ToString();
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetJQueryNameValuePairs(this FormDataCollection formData)
        {
            if (formData == null)
                throw Error.ArgumentNull("formData");
            int count = 0;
            foreach (KeyValuePair<string, string> keyValuePair in formData)
            {
                FormDataCollectionExtensions.ThrowIfMaxHttpCollectionKeysExceeded(count);
                string key = FormDataCollectionExtensions.NormalizeJQueryToMvc(keyValuePair.Key);
                string value = keyValuePair.Value ?? string.Empty;
                yield return new KeyValuePair<string, string>(key, value);
                ++count;
            }
        }

        private static void ThrowIfMaxHttpCollectionKeysExceeded(int count)
        {
            if (count >= MediaTypeFormatter.MaxHttpCollectionKeys)
                throw Error.InvalidOperation("SRResources.MaxHttpCollectionKeyLimitReached", new object[2]
                {
          (object) MediaTypeFormatter.MaxHttpCollectionKeys,
          (object) typeof (MediaTypeFormatter)
                });
        }

        //internal static IValueProvider GetJQueryValueProvider(this FormDataCollection formData)
        //{
        //    if (formData == null)
        //        throw Error.ArgumentNull("formData");
        //    return (IValueProvider)new NameValuePairsValueProvider(FormDataCollectionExtensions.GetJQueryNameValuePairs(formData), CultureInfo.InvariantCulture);
        //}

        /// <summary>
        /// Reads the collection extensions with specified type.
        /// </summary>
        /// 
        /// <returns>
        /// The read collection extensions.
        /// </returns>
        /// <param name="formData">The form data.</param><typeparam name="T">The generic type.</typeparam>
        //public static T ReadAs<T>(this FormDataCollection formData)
        //{
        //    return (T)FormDataCollectionExtensions.ReadAs(formData, typeof(T));
        //}

        /// <typeparam name="T"/>
        //public static T ReadAs<T>(this FormDataCollection formData, HttpActionContext actionContext)
        //{
        //    return (T)FormDataCollectionExtensions.ReadAs(formData, typeof(T), string.Empty, actionContext);
        //}

        ///// <summary>
        ///// Reads the collection extensions with specified type.
        ///// </summary>
        ///// 
        ///// <returns>
        ///// The collection extensions with specified type.
        ///// </returns>
        ///// <param name="formData">The form data.</param><param name="type">The type of the object.</param>
        //public static object ReadAs(this FormDataCollection formData, Type type)
        //{
        //    return FormDataCollectionExtensions.ReadAs(formData, type, string.Empty, (IRequiredMemberSelector)null, (IFormatterLogger)null);
        //}

        //public static object ReadAs(this FormDataCollection formData, Type type, HttpActionContext actionContext)
        //{
        //    return FormDataCollectionExtensions.ReadAs(formData, type, string.Empty, actionContext);
        //}

        ///// <summary>
        ///// Reads the collection extensions with specified type.
        ///// </summary>
        ///// 
        ///// <returns>
        ///// The collection extensions.
        ///// </returns>
        ///// <param name="formData">The form data.</param><param name="modelName">The name of the model.</param><param name="requiredMemberSelector">The required member selector.</param><param name="formatterLogger">The formatter logger.</param><typeparam name="T">The generic type.</typeparam>
        //public static T ReadAs<T>(this FormDataCollection formData, string modelName, IRequiredMemberSelector requiredMemberSelector, IFormatterLogger formatterLogger)
        //{
        //    return (T)FormDataCollectionExtensions.ReadAs(formData, typeof(T), modelName, requiredMemberSelector, formatterLogger);
        //}

        ///// <typeparam name="T"/>
        //public static T ReadAs<T>(this FormDataCollection formData, string modelName, HttpActionContext actionContext)
        //{
        //    return (T)FormDataCollectionExtensions.ReadAs(formData, typeof(T), modelName, actionContext);
        //}

        //public static object ReadAs(this FormDataCollection formData, Type type, string modelName, HttpActionContext actionContext)
        //{
        //    if (formData == null)
        //        throw Error.ArgumentNull("formData");
        //    if (type == (Type)null)
        //        throw Error.ArgumentNull("type");
        //    if (actionContext == null)
        //        throw Error.ArgumentNull("actionContext");
        //    return FormDataCollectionExtensions.ReadAsInternal(formData, type, modelName, actionContext);
        //}

        /// <summary>
        /// Reads the collection extensions with specified type and model name.
        /// </summary>
        /// 
        /// <returns>
        /// The collection extensions.
        /// </returns>
        /// <param name="formData">The form data.</param><param name="type">The type of the object.</param><param name="modelName">The name of the model.</param><param name="requiredMemberSelector">The required member selector.</param><param name="formatterLogger">The formatter logger.</param>
        //public static object ReadAs(this FormDataCollection formData, Type type, string modelName, IRequiredMemberSelector requiredMemberSelector, IFormatterLogger formatterLogger)
        //{
        //    return FormDataCollectionExtensions.ReadAs(formData, type, modelName, requiredMemberSelector, formatterLogger, (HttpConfiguration)null);
        //}

        /// <summary>
        /// Deserialize the form data to the given type, using model binding.
        /// </summary>
        /// 
        /// <returns>
        /// best attempt to bind the object. The best attempt may be null.
        /// </returns>
        /// <param name="formData">collection with parsed form url data</param><param name="type">target type to read as</param><param name="modelName">null or empty to read the entire form as a single object. This is common for body data. Or the name of a model to do a partial binding against the form data. This is common for extracting individual fields.</param><param name="requiredMemberSelector">The <see cref="T:System.Net.Http.Formatting.IRequiredMemberSelector"/> used to determine required members.</param><param name="formatterLogger">The <see cref="T:System.Net.Http.Formatting.IFormatterLogger"/> to log events to.</param><param name="config">The <see cref="T:System.Web.Http.HttpConfiguration"/> configuration to pick binder from. Can be null if the config was not created already. In that case a new config is created.</param>
        //public static object ReadAs(this FormDataCollection formData, Type type, string modelName, IRequiredMemberSelector requiredMemberSelector, IFormatterLogger formatterLogger, HttpConfiguration config)
        //{
        //    if (formData == null)
        //        throw Error.ArgumentNull("formData");
        //    if (type == (Type)null)
        //        throw Error.ArgumentNull("type");
        //    object obj = (object)null;
        //    HttpActionContext actionContext = (HttpActionContext)null;
        //    if (requiredMemberSelector != null && formatterLogger != null)
        //    {
        //        using (HttpConfiguration config1 = new HttpConfiguration())
        //        {
        //            config = config == null ? config1 : config;
        //            config1.Services = (ServicesContainer)new FormDataCollectionExtensions.ServicesContainerWrapper(config, (ModelValidatorProvider)new RequiredMemberModelValidatorProvider(requiredMemberSelector));
        //            actionContext = FormDataCollectionExtensions.CreateActionContextForModelBinding(config1);
        //            obj = FormDataCollectionExtensions.ReadAs(formData, type, modelName, actionContext);
        //        }
        //    }
        //    else if (config == null)
        //    {
        //        using (config = new HttpConfiguration())
        //        {
        //            actionContext = FormDataCollectionExtensions.CreateActionContextForModelBinding(config);
        //            obj = FormDataCollectionExtensions.ReadAs(formData, type, modelName, actionContext);
        //        }
        //    }
        //    else
        //    {
        //        actionContext = FormDataCollectionExtensions.CreateActionContextForModelBinding(config);
        //        obj = FormDataCollectionExtensions.ReadAs(formData, type, modelName, actionContext);
        //    }
        //    if (formatterLogger != null)
        //    {
        //        foreach (KeyValuePair<string, ModelState> keyValuePair in actionContext.ModelState)
        //        {
        //            foreach (ModelError modelError in (Collection<ModelError>)keyValuePair.Value.Errors)
        //            {
        //                if (modelError.Exception != null)
        //                    formatterLogger.LogError(keyValuePair.Key, modelError.Exception);
        //                else
        //                    formatterLogger.LogError(keyValuePair.Key, modelError.ErrorMessage);
        //            }
        //        }
        //    }
        //    return obj;
        //}

        //private static object ReadAsInternal(this FormDataCollection formData, Type type, string modelName, HttpActionContext actionContext)
        //{
        //    IValueProvider jqueryValueProvider = FormDataCollectionExtensions.GetJQueryValueProvider(formData);
        //    ModelBindingContext modelBindingContext = FormDataCollectionExtensions.CreateModelBindingContext(actionContext, modelName ?? string.Empty, type, jqueryValueProvider);
        //    if (FormDataCollectionExtensions.CreateModelBindingProvider(actionContext).GetBinder(actionContext.ControllerContext.Configuration, type).BindModel(actionContext, modelBindingContext))
        //        return modelBindingContext.Model;
        //    return MediaTypeFormatter.GetDefaultValueForType(type);
        //}

        //private static ModelBinderProvider CreateModelBindingProvider(HttpActionContext actionContext)
        //{
        //    return (ModelBinderProvider)new CompositeModelBinderProvider(ServicesExtensions.GetModelBinderProviders(actionContext.ControllerContext.Configuration.Services));
        //}

        //private static ModelBindingContext CreateModelBindingContext(HttpActionContext actionContext, string modelName, Type type, IValueProvider vp)
        //{
        //    ModelMetadataProvider metadataProvider = ServicesExtensions.GetModelMetadataProvider(actionContext.ControllerContext.Configuration.Services);
        //    return new ModelBindingContext()
        //    {
        //        ModelName = modelName,
        //        FallbackToEmptyPrefix = false,
        //        ModelMetadata = metadataProvider.GetMetadataForType((Func<object>)null, type),
        //        ModelState = actionContext.ModelState,
        //        ValueProvider = vp
        //    };
        //}

        //private static HttpActionContext CreateActionContextForModelBinding(HttpConfiguration config)
        //{
        //    HttpControllerContext controllerContext = new HttpControllerContext()
        //    {
        //        Configuration = config
        //    };
        //    controllerContext.ControllerDescriptor = new HttpControllerDescriptor(config);
        //    return new HttpActionContext()
        //    {
        //        ControllerContext = controllerContext
        //    };
        //}
    }
}
