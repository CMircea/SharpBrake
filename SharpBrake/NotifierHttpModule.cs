using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SharpBrake
{
    /// <summary>
    /// The <see cref="IHttpModule"/> that notifies Airbrake of unhandled exceptions in the application.
    /// </summary>
    public sealed class NotifierHttpModule : IHttpModule
    {
        /*\ ****** ****** ****** ****** ****** Public Methods ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// 
        /// <param name="application">
        ///     An <see cref="T:System.Web.HttpApplication"/> that provides access to the methods, properties,
        ///     and events common to all application objects within an ASP.NET application.
        /// </param>
        public void Init(HttpApplication application)
        {
            application.Error += ApplicationOnError;
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public void Dispose()
        {

        }

        /*\ ****** ****** ****** ****** ****** Private Methods ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Notifies Airbrake of the unhandled application error that occurred.
        /// </summary>
        /// 
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ApplicationOnError(object sender, EventArgs e)
        {
            var application = (HttpApplication) sender;
            var exception   = application.Server.GetLastError();

            // Do not log HTTP errors.
            if (exception is HttpException)
                return;

            exception.SendToAirbrake();
        }
    }
}
