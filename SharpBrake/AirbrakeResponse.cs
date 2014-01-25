using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Common.Logging;
using SharpBrake.Serialization;

namespace SharpBrake
{
    /// <summary>
    /// The response received from Airbrake.
    /// </summary>
    public sealed class AirbrakeResponse
    {
        /*\ ****** ****** ****** ****** ****** Properties ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Gets the content.
        /// </summary>
        public string Content
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// 
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IList<AirbrakeResponseError> Errors
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the headers.
        /// </summary>
        public WebHeaderCollection Headers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is from cache.
        /// </summary>
        /// 
        /// <value>
        /// <c>true</c> if this instance is from cache; otherwise, <c>false</c>.
        /// </value>
        public bool IsFromCache
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is mutually authenticated.
        /// </summary>
        /// 
        /// <value>
        /// <c>true</c> if this instance is mutually authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsMutuallyAuthenticated
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the notice returned from Airbrake.
        /// </summary>
        public AirbrakeResponseNotice Notice
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the response URI.
        /// </summary>
        public Uri ResponseUri
        {
            get;
            private set;
        }

        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeResponse"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="content">The content.</param>
        public AirbrakeResponse(WebResponse response, string content)
        {
            Content = content;
            Errors  = new List<AirbrakeResponseError>();

            if (response != null)
            {
                // TryGet is needed because the default behavior of WebResponse is to throw NotImplementedException
                // when a method isn't overridden by a deriving class, instead of declaring the method as abstract.
                ContentType             = response.TryGet(x => x.ContentType);
                Headers                 = response.TryGet(x => x.Headers);
                IsFromCache             = response.TryGet(x => x.IsFromCache);
                IsMutuallyAuthenticated = response.TryGet(x => x.IsMutuallyAuthenticated);
                ResponseUri             = response.TryGet(x => x.ResponseUri);
            }

            try
            {
                Deserialize(content);
            }
            catch (Exception exception)
            {
                LogManager.GetLogger(GetType())
                          .Fatal(f => f("An error occurred while deserializing the following content:\n{0}", content), exception);
            }
        }

        /*\ ****** ****** ****** ****** ****** Private Methods ****** ****** ****** ****** ****** \*/
        private void Deserialize(string xml)
        {
            using (var stringReader = new StringReader(xml))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    reader.MoveToContent();

                    switch (reader.LocalName)
                    {
                        case "errors":
                            Errors = reader.BuildErrors().ToList();

                            break;

                        case "notice":
                            Notice = reader.BuildNotice();

                            break;
                    }
                }
            }
        }
    }
}
