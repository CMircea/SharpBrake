using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace SharpBrake
{
    /// <summary>
    /// Configuration class for Airbrake.
    /// </summary>
    public class AirbrakeConfiguration
    {
        /*\ ****** ****** ****** ****** ****** Properties ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// 
        /// <value>
        /// The API key.
        /// </value>
        public string ApiKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// 
        /// <value>
        /// The application version.
        /// </value>
        public string AppVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Airbrake server endpoint.
        /// </summary>
        public string Endpoint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the environment.
        /// </summary>
        /// 
        /// <value>
        /// The name of the environment.
        /// </value>
        public string EnvironmentName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the project root. By default set to <see cref="HttpRequest.ApplicationPath"/>
        /// if <see cref="HttpContext.Current"/> is not null, else <see cref="System.Environment.CurrentDirectory"/>. 
        /// </summary>
        /// 
        /// <remarks>
        /// Only set this if you need to override the default project root.
        /// </remarks>
        /// 
        /// <value>
        /// The project root.
        /// </value>
        public string ProjectRoot
        {
            get;
            set;
        }

        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeConfiguration"/> class.
        /// </summary>
        public AirbrakeConfiguration()
        {
            ApiKey          = ConfigurationManager.AppSettings["Airbrake.ApiKey"];
            Endpoint        = ConfigurationManager.AppSettings["Airbrake.Endpoint"];
            EnvironmentName = ConfigurationManager.AppSettings["Airbrake.Environment"];
            ProjectRoot     = HttpRuntime.AppDomainAppVirtualPath ?? Environment.CurrentDirectory;

            string[] values = ConfigurationManager.AppSettings.GetValues("Airbrake.AppVersion");
            
            if (values != null)
                AppVersion = values.FirstOrDefault();
        }
    }
}
