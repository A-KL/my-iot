using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web.Http.Routing;

namespace System.Web.Http
{
    [DefaultMember("Item")]
    public class HttpRouteCollection// : ICollection<IHttpRoute>, IEnumerable<IHttpRoute>, IEnumerable, IDisposable
    {
        //        private static readonly Uri referenceBaseAddress = new Uri("http://localhost");
        //        private readonly List<IHttpRoute> collection = new List<IHttpRoute>();
        //        private readonly IDictionary<string, IHttpRoute> dictionary = (IDictionary<string, IHttpRoute>)new Dictionary<string, IHttpRoute>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
        //        private readonly string virtualPathRoot;
        //        private bool disposed;

        //        /// <summary>
        //        /// Gets the virtual path root.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// The virtual path root.
        //        /// </returns>
        //        public virtual string VirtualPathRoot
        //        {
        //            get
        //            {
        //                return this.virtualPathRoot;
        //            }
        //        }

        //        /// <summary>
        //        /// Gets the number of items in the collection.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// The number of items in the collection.
        //        /// </returns>
        //        public virtual int Count
        //        {
        //            get
        //            {
        //                return this.collection.Count;
        //            }
        //        }

        //        /// <summary>
        //        /// Gets a value indicating whether the collection is read-only.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// true if the collection is read-only; otherwise, false.
        //        /// </returns>
        //        public virtual bool IsReadOnly
        //        {
        //            get
        //            {
        //                return false;
        //            }
        //        }

        //        /// <summary>
        //        /// Gets or sets the element at the specified index.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// The  <see cref="T:System.Web.Http.Routing.IHttpRoute"/> at the specified index.
        //        /// </returns>
        //        /// <param name="index">The index.</param>
        //        public virtual IHttpRoute this[int index]
        //        {
        //            get
        //            {
        //                return this.collection[index];
        //            }
        //        }

        //        /// <summary>
        //        /// Gets or sets the element with the specified route name.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// The  <see cref="T:System.Web.Http.Routing.IHttpRoute"/> at the specified index.
        //        /// </returns>
        //        /// <param name="name">The route name.</param>
        //        public virtual IHttpRoute this[string name]
        //        {
        //            get
        //            {
        //                return this.dictionary[name];
        //            }
        //        }

        //        /// <summary>
        //        /// Initializes a new instance of the <see cref="T:System.Web.Http.HttpRouteCollection"/> class.
        //        /// </summary>
        //        public HttpRouteCollection()
        //          : this("/")
        //        {
        //        }

        //        /// <summary>
        //        /// Initializes a new instance of the <see cref="T:System.Web.Http.HttpRouteCollection"/> class.
        //        /// </summary>
        //        /// <param name="virtualPathRoot">The virtual path root.</param>
        //        public HttpRouteCollection(string virtualPathRoot)
        //        {
        //            if (virtualPathRoot == null)
        //            {
        //                throw Error.ArgumentNull("virtualPathRoot");
        //            }
        //            this.virtualPathRoot = "/" + new Uri(HttpRouteCollection.referenceBaseAddress, virtualPathRoot).GetComponents(UriComponents.Path, UriFormat.Unescaped);
        //        }

        //        /// <summary>
        //        /// Gets the route data for a specified HTTP request.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// An<see cref="T:System.Web.Http.Routing.IHttpRouteData"/> instance that represents the route data.
        //        /// </returns>
        //        /// <param name="request">The HTTP request.</param>
        //        public virtual IHttpRouteData GetRouteData(HttpRequestMessage request)
        //        {
        //            if (request == null)
        //                throw Error.ArgumentNull("request");
        //            for (int index = 0; index < this._collection.Count; ++index)
        //            {
        //                string virtualPathRoot = this.GetVirtualPathRoot(HttpRequestMessageExtensions.GetRequestContext(request));
        //                IHttpRouteData routeData = this.collection[index].GetRouteData(virtualPathRoot, request);
        //                if (routeData != null)
        //                    return routeData;
        //            }
        //            return null;
        //        }

        //        /// <summary>
        //        /// Gets a virtual path.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// An <see cref="T:System.Web.Http.Routing.IHttpVirtualPathData"/> instance that represents the virtual path.
        //        /// </returns>
        //        /// <param name="request">The HTTP request.</param><param name="name">The route name.</param><param name="values">The route values.</param>
        //        public virtual IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, string name, IDictionary<string, object> values)
        //        {
        //            if (request == null)
        //                throw Error.ArgumentNull("request");

        //            if (name == null)
        //                throw Error.ArgumentNull("name");

        //            IHttpRoute httpRoute;

        //            if (!this._dictionary.TryGetValue(name, out httpRoute))
        //                throw Error.Argument("name", SRResources.RouteCollection_NameNotFound, (object)name);

        //            var virtualPath = httpRoute.GetVirtualPath(request, values);

        //            if (virtualPath == null)
        //                return null;

        //            string virtualPathRoot = this.GetVirtualPathRoot(HttpRequestMessageExtensions.GetRequestContext(request));

        //            if (!virtualPathRoot.EndsWith("/", StringComparison.Ordinal))
        //                virtualPathRoot += "/";

        //            return (IHttpVirtualPathData)new HttpVirtualPathData(virtualPath.Route, virtualPathRoot + virtualPath.VirtualPath);
        //        }

        //        private string GetVirtualPathRoot(HttpRequestContext requestContext)
        //        {
        //            if (requestContext != null)
        //                return requestContext.VirtualPathRoot ?? string.Empty;

        //            return this.virtualPathRoot;
        //        }

        //        /// <summary>
        //        /// Creates an <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// The new <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance.
        //        /// </returns>
        //        /// <param name="routeTemplate">The route template.</param><param name="defaults">An object that contains the default route parameters.</param><param name="constraints">An object that contains the route constraints.</param>
        //        public IHttpRoute CreateRoute(string routeTemplate, object defaults, object constraints)
        //        {
        //            var dataTokens = new Dictionary<string, object>();

        //            return this.CreateRoute(routeTemplate, (IDictionary<string, object>)new HttpRouteValueDictionary(defaults), (IDictionary<string, object>)new HttpRouteValueDictionary(constraints), dataTokens, (HttpMessageHandler)null);
        //        }

        //        /// <summary>
        //        /// Creates an <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// The new <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance.
        //        /// </returns>
        //        /// <param name="routeTemplate">The route template.</param><param name="defaults">An object that contains the default route parameters.</param><param name="constraints">An object that contains the route constraints.</param><param name="dataTokens">The route data tokens.</param>
        //        public IHttpRoute CreateRoute(string routeTemplate, IDictionary<string, object> defaults, IDictionary<string, object> constraints, IDictionary<string, object> dataTokens)
        //        {
        //            return this.CreateRoute(routeTemplate, defaults, constraints, dataTokens, (HttpMessageHandler)null);
        //        }

        //        /// <summary>
        //        /// Creates an <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// The new <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance.
        //        /// </returns>
        //        /// <param name="routeTemplate">The route template.</param><param name="defaults">An object that contains the default route parameters.</param><param name="constraints">An object that contains the route constraints.</param><param name="dataTokens">The route data tokens.</param><param name="handler">The message handler for the route.</param>
        //        public virtual IHttpRoute CreateRoute(string routeTemplate, IDictionary<string, object> defaults, IDictionary<string, object> constraints, IDictionary<string, object> dataTokens, HttpMessageHandler handler)
        //        {
        //            HttpRouteValueDictionary defaults1 = new HttpRouteValueDictionary(defaults);
        //            HttpRouteValueDictionary constraints1 = new HttpRouteValueDictionary(constraints);
        //            HttpRouteValueDictionary dataTokens1 = new HttpRouteValueDictionary(dataTokens);

        //            foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) constraints1)
        //            {
        //                this.ValidateConstraint(routeTemplate, keyValuePair.Key, keyValuePair.Value);
        //            }

        //            return (IHttpRoute)new HttpRoute(routeTemplate, defaults1, constraints1, dataTokens1, handler);
        //        }

        //        /// <summary>
        //        /// Validates that a constraint is valid for an <see cref="T:System.Web.Http.Routing.IHttpRoute"/> created by a call to the <see cref="M:System.Web.Http.HttpRouteCollection.CreateRoute(System.String,System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object},System.Collections.Generic.IDictionary{System.String,System.Object},System.Net.Http.HttpMessageHandler)"/> method.
        //        /// </summary>
        //        /// <param name="routeTemplate">The route template.</param><param name="name">The constraint name.</param><param name="constraint">The constraint object.</param>
        //        protected virtual void ValidateConstraint(string routeTemplate, string name, object constraint)
        //        {
        //            if (name == null)
        //                throw Error.ArgumentNull("name");
        //            if (constraint == null)
        //                throw Error.ArgumentNull("constraint");

        //            HttpRoute.ValidateConstraint(routeTemplate, name, constraint);
        //        }

        //        void ICollection<IHttpRoute>.Add(IHttpRoute route)
        //        {
        //            throw Error.NotSupported(SRResources.Route_AddRemoveWithNoKeyNotSupported, (object)typeof(HttpRouteCollection).Name);
        //        }

        //        /// <summary>
        //        /// Adds an <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance to the collection.
        //        /// </summary>
        //        /// <param name="name">The name of the route.</param><param name="route">The <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance to add to the collection.</param>
        //        public virtual void Add(string name, IHttpRoute route)
        //        {
        //            if (name == null)
        //                throw Error.ArgumentNull("name");

        //            if (route == null)
        //                throw Error.ArgumentNull("route");

        //            this.dictionary.Add(name, route);
        //            this.collection.Add(route);
        //        }

        //        /// <summary>
        //        /// Removes all items from  the collection.
        //        /// </summary>
        //        public virtual void Clear()
        //        {
        //            this.dictionary.Clear();
        //            this.collection.Clear();
        //        }

        //        /// <summary>
        //        /// Determines whether the collection contains a specific <see cref="T:System.Web.Http.Routing.IHttpRoute"/>.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// true if the <see cref="T:System.Web.Http.Routing.IHttpRoute"/> is found in the collection; otherwise, false.
        //        /// </returns>
        //        /// <param name="item">The object to locate in the collection.</param>
        //        public virtual bool Contains(IHttpRoute item)
        //        {
        //            if (item == null)
        //                throw Error.ArgumentNull("item");

        //            return this.collection.Contains(item);
        //        }

        //        /// <summary>
        //        /// Determines whether the collection contains an element with the specified key.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// true if the collection contains an element with the key; otherwise, false.
        //        /// </returns>
        //        /// <param name="name">The key to locate in the collection.</param>
        //        public virtual bool ContainsKey(string name)
        //        {
        //            if (name == null)
        //                throw Error.ArgumentNull("name");

        //            return this.dictionary.ContainsKey(name);
        //        }

        //        /// <summary>
        //        /// Copies the route names and <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instances of the collection to an array, starting at a particular array index.
        //        /// </summary>
        //        /// <param name="array">The array that is the destination of the elements copied from the collection.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        //        public virtual void CopyTo(IHttpRoute[] array, int arrayIndex)
        //        {
        //            this.collection.CopyTo(array, arrayIndex);
        //        }

        //        /// <summary>
        //        /// Copies the <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instances of the collection to an array, starting at a particular array index.
        //        /// </summary>
        //        /// <param name="array">The array that is the destination of the elements copied from the collection.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        //        public virtual void CopyTo(KeyValuePair<string, IHttpRoute>[] array, int arrayIndex)
        //        {
        //            this.dictionary.CopyTo(array, arrayIndex);
        //        }

        //        /// <summary>
        //        /// Inserts an <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance into the collection.
        //        /// </summary>
        //        /// <param name="index">The zero-based index at which <paramref name="value"/> should be inserted.</param><param name="name">The route name.</param><param name="value">The <see cref="T:System.Web.Http.Routing.IHttpRoute"/> to insert. The value cannot be null.</param>
        //        public virtual void Insert(int index, string name, IHttpRoute value)
        //        {
        //            if (name == null)
        //                throw Error.ArgumentNull("name");

        //            if (value == null)
        //                throw Error.ArgumentNull("value");

        //            if (this.collection[index] == null)
        //                return;

        //            this.dictionary.Add(name, value);

        //            this.collection.Insert(index, value);
        //        }

        //        bool ICollection<IHttpRoute>.Remove(IHttpRoute route)
        //        {
        //            throw Error.NotSupported(SRResources.Route_AddRemoveWithNoKeyNotSupported, (object)typeof(HttpRouteCollection).Name);
        //        }

        //        /// <summary>
        //        /// Removes an <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance from the collection.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// true if the element is successfully removed; otherwise, false. This method also returns false if <paramref name="name"/> was not found in the collection.
        //        /// </returns>
        //        /// <param name="name">The name of the route to remove.</param>
        //        public virtual bool Remove(string name)
        //        {
        //            if (name == null)
        //                throw Error.ArgumentNull("name");

        //            IHttpRoute httpRoute;

        //            if (!this.dictionary.TryGetValue(name, out httpRoute))
        //                return false;

        //            var flag = this.dictionary.Remove(name);

        //            this.collection.Remove(httpRoute);

        //            return flag;
        //        }

        //        /// <summary>
        //        /// Returns an enumerator that iterates through the collection.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// An <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        //        /// </returns>
        //        public virtual IEnumerator<IHttpRoute> GetEnumerator()
        //        {
        //            return (IEnumerator<IHttpRoute>)this.collection.GetEnumerator();
        //        }

        //        IEnumerator IEnumerable.GetEnumerator()
        //        {
        //            return this.OnGetEnumerator();
        //        }

        //        /// <summary>
        //        /// Called internally to get the enumerator for the collection.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// An <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the collection.
        //        /// </returns>
        //        protected virtual IEnumerator OnGetEnumerator()
        //        {
        //            return (IEnumerator)this.collection.GetEnumerator();
        //        }

        //        /// <summary>
        //        /// Gets the <see cref="T:System.Web.Http.Routing.IHttpRoute"/> with the specified route name.
        //        /// </summary>
        //        /// 
        //        /// <returns>
        //        /// true if the collection contains an element with the specified name; otherwise, false.
        //        /// </returns>
        //        /// <param name="name">The route name.</param><param name="route">When this method returns, contains the <see cref="T:System.Web.Http.Routing.IHttpRoute"/> instance, if the route name is found; otherwise, null. This parameter is passed uninitialized.</param>
        //        public virtual bool TryGetValue(string name, out IHttpRoute route)
        //        {
        //            return this.dictionary.TryGetValue(name, out route);
        //        }

        //        /// <summary>
        //        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        //        /// </summary>
        //        public void Dispose()
        //        {
        //            this.Dispose(true);
        //            GC.SuppressFinalize((object)this);
        //        }

        //        /// <summary>
        //        /// Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.
        //        /// </summary>
        //        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        //        protected virtual void Dispose(bool disposing)
        //        {
        //            if (this.disposed)
        //                return;

        //            if (disposing)
        //            {
        //                var hashSet = new HashSet<IDisposable>();

        //                foreach (var httpRoute in this)
        //                {
        //                    if (httpRoute.Handler != null)
        //                        hashSet.Add(httpRoute.Handler);
        //                }

        //                foreach (var disposable in hashSet)
        //                {
        //                    disposable.Dispose();
        //                }
        //            }

        //            this.disposed = true;
        //        }
    }
}
