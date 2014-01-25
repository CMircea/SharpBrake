using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpBrake.Serialization
{
    /// <summary>
    /// Wraps XML serialization and doesn't generate processing instructions on document start 
    /// as well as xsi and xsd namespace definitions
    /// </summary>
    /// 
    /// <typeparam name="T">The type object to serialize.</typeparam>
    public sealed class CleanXmlSerializer<T> : CleanXmlSerializer
    {
        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="CleanXmlSerializer{TRoot}"/> class.
        /// </summary>
        public CleanXmlSerializer()
            : base(typeof(T))
        {

        }

        /*\ ****** ****** ****** ****** ****** Public Methods ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Serializes the <paramref name="source"/> to XML.
        /// </summary>
        /// 
        /// <param name="source">The source.</param>
        /// 
        /// <returns>
        /// The <paramref name="source"/> serialized to XML.
        /// </returns>
        public string ToXml(T source)
        {
            return ToXml((object)source);
        }
    }
}
