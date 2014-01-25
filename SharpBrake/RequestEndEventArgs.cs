using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpBrake
{
    /// <summary>
    /// The event arguments passed to <see cref="EventHandler{RequestEndEventArgs}"/>.
    /// </summary>
    [Serializable]
    public class RequestEndEventArgs : EventArgs
    {
        /*\ ****** ****** ****** ****** ****** Properties ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Gets the request.
        /// </summary>
        public WebRequest Request
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        public AirbrakeResponse Response
        {
            get;
            private set;
        }

        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestEndEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        /// <param name="content">The body of the response.</param>
        public RequestEndEventArgs(WebRequest request, WebResponse response, string content)
        {
            Request  = request;
            Response = new AirbrakeResponse(response, content);
        }
    }
}
