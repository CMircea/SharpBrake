using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBrake
{
    /// <summary>
    /// Contains the error message returned from Airbrake.
    /// </summary>
    public sealed class AirbrakeResponseError
    {
        /*\ ****** ****** ****** ****** ****** Properties ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeResponseError"/> class.
        /// </summary>
        /// 
        /// <param name="message">The message.</param>
        public AirbrakeResponseError(string message)
        {
            Message = message;
        }

        /*\ ****** ****** ****** ****** ****** Public Methods ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Message;
        }
    }
}
