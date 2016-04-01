// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Net.Http;

namespace System.Web.Http.Controllers
{
    /// <summary>
    /// Contains information for a single HTTP operation.
    /// </summary>
    public class HttpControllerContext
    {
        private HttpRequestContext _requestContext;
        private HttpRequestMessage _request;
       // private HttpControllerDescriptor _controllerDescriptor;
        private IHttpController _controller;

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// 
        /// <returns>
        /// The configuration.
        /// </returns>
        public HttpConfiguration Configuration
        {
            get
            {
                return this._requestContext.Configuration;
            }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }
                this._requestContext.Configuration = value;
            }
        }

        public HttpRequestMessage Request
        {
            get { return _request; }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }

                _request = value;
            }
        }

        /// <summary>
        /// Gets or sets the request context.
        /// </summary>
        public HttpRequestContext RequestContext
        {
            get
            {
                return this._requestContext;
            }
            set
            {
                if (value == null)
                    throw Error.PropertyNull();
                this._requestContext = value;
            }
        }

        /// <summary>
        /// Gets or sets the controller descriptor.
        /// </summary>
        /// <value>
        /// The controller descriptor.
        /// </value>
        //public HttpControllerDescriptor ControllerDescriptor
        //{
        //    get { return _controllerDescriptor; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            throw Error.PropertyNull();
        //        }

        //        _controllerDescriptor = value;
        //    }
        //}

        /// <summary>
        /// Gets or sets the HTTP controller.
        /// </summary>
        /// <value>
        /// The HTTP controller.
        /// </value>
        public IHttpController Controller
        {
            get { return _controller; }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }

                _controller = value;
            }
        }
    }
}
