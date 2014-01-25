using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SharpBrake.Serialization
{
    /// <summary>
    /// Optional. If this error occurred during an HTTP request, the children of this element can be used to describe the request that caused the error.
    /// </summary>
    [XmlInclude(typeof(AirbrakeVar))]
    public sealed class AirbrakeRequest
    {
        private AirbrakeVar[] _cgiData;
        private AirbrakeVar[] _parameters;
        private AirbrakeVar[] _session;

        /*\ ****** ****** ****** ****** ****** Properties ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeRequest"/> class.
        /// </summary>
        /// 
        /// <param name="url">The URL.</param>
        /// <param name="component">
        /// The component in which the error occurred.
        /// In model-view-controller frameworks like Rails, this should be set to the controller.
        /// Otherwise, this can be set to a route or other request category.
        /// </param>
        public AirbrakeRequest(string url, string component)
        {
            Url       = url;
            Component = component;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeRequest"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="component">
        /// The component in which the error occurred.
        /// In model-view-controller frameworks like Rails, this should be set to the controller.
        /// Otherwise, this can be set to a route or other request category.
        /// </param>
        public AirbrakeRequest(Uri url, string component)
        {
            Url       = url == null ? null : url.ToString();
            Component = component;
        }


        /// <summary>
        /// Optional. The action in which the error occurred.
        /// If each request is routed to a controller action, this should be set here.
        /// Otherwise, this can be set to a method or other request subcategory.
        /// </summary>
        /// 
        /// <value>
        /// The action in which the error occurred.
        /// </value>
        [XmlElement("action")]
        public string Action
        {
            get;
            set;
        }

        /// <summary>
        /// Optional. A list of var elements describing CGI variables from the request, such as SERVER_NAME and REQUEST_URI.
        /// </summary>
        /// 
        /// <value>
        /// A list of var elements describing CGI variables from the request.
        /// </value>
        [XmlArray("cgi-data")]
        [XmlArrayItem("var")]
        public AirbrakeVar[] CgiData
        {
            get
            {
                if (_cgiData != null && _cgiData.Any())
                {
                    return _cgiData;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _cgiData = value;
            }
        }

        /// <summary>
        /// Required only if there is a request element.
        /// The component in which the error occurred.
        /// In model-view-controller frameworks like Rails, this should be set to the controller.
        /// Otherwise, this can be set to a route or other request category.
        /// </summary>
        /// 
        /// <value>
        /// The component in which the error occurred.
        /// </value>
        [XmlElement("component")]
        public string Component
        {
            get;
            set;
        }

        /// <summary>
        /// Optional. A list of var elements describing request parameters from the query string, POST body, routing, and other inputs.
        /// </summary>
        /// 
        /// <value>
        /// A list of var elements describing request parameters.
        /// </value>
        [XmlArray("params")]
        [XmlArrayItem("var")]
        public AirbrakeVar[] Params
        {
            get
            {
                if (_parameters != null && _parameters.Any())
                {
                    return _parameters;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _parameters = value;
            }
        }

        /// <summary>
        /// Optional. A list of var elements describing session variables from the request.
        /// </summary>
        /// 
        /// <value>
        /// A list of var elements describing session variables from the request.
        /// </value>
        [XmlArray("session")]
        [XmlArrayItem("var")]
        public AirbrakeVar[] Session
        {
            get
            {
                if (_session != null && _session.Any())
                {
                    return _session;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _session = value;
            }
        }

        /// <summary>
        /// Required only if there is a request element. The URL at which the error occurred.
        /// </summary>
        /// <value>
        /// The URL at which the error occurred.
        /// </value>
        [XmlElement("url")]
        public string Url
        {
            get;
            set;
        }

        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeRequest"/> class.
        /// </summary>
        internal AirbrakeRequest()
        {

        }
    }
}
