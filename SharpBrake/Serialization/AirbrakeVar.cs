using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SharpBrake.Serialization
{
    /// <summary>
    /// The params, session, and cgi-data elements can contain one or more var elements for each parameter or variable that was set when the error occurred.
    /// Each var element should have a @key attribute for the name of the variable, and element text content for the value of the variable.
    /// </summary>
    [XmlRoot("var")]
    public sealed class AirbrakeVar : IEquatable<AirbrakeVar>
    {
        /*\ ****** ****** ****** ****** ****** Properties ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Required. The key of the var, like <c>SERVER_NAME</c> or <c>REQUEST_URI</c>.
        /// </summary>
        /// 
        /// <value>
        /// The key of the var, like <c>SERVER_NAME</c> or <c>REQUEST_URI</c>.
        /// </value>
        [XmlAttribute("key")]
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// 
        /// <value>
        /// The value.
        /// </value>
        [XmlText]
        public string Value
        {
            get;
            set;
        }

        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeVar"/> class.
        /// </summary>
        internal AirbrakeVar()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeVar"/> class.
        /// </summary>
        /// 
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public AirbrakeVar(string key, object value)
        {
            Key   = key;
            Value = value != null ? value.ToString() : null;
        }

        /*\ ****** ****** ****** ****** ****** Operators ****** ****** ****** ****** ****** \*/
        public static bool operator ==(AirbrakeVar left, AirbrakeVar right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AirbrakeVar left, AirbrakeVar right)
        {
            return !Equals(left, right);
        }

        /*\ ****** ****** ****** ****** ****** Public Methods ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(AirbrakeVar other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return String.Equals(Key, other.Key) && String.Equals(Value, other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// 
        /// <param name="other">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return other is AirbrakeVar && Equals((AirbrakeVar) other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// 
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("[{0} : {1}]", Key, Value);
        }
    }
}
