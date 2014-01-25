using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SharpBrake.Serialization
{
    /// <summary>
    /// Wraps XML serialization and doesn't generate processing instructions on document start 
    /// as well as xsi and xsd namespace definitions
    /// </summary>
    public abstract class CleanXmlSerializer
    {
        private readonly XmlSerializer _serializer;
        private readonly XmlSerializerNamespaces _namespaces;

        /*\ ****** ****** ****** ****** ****** Classes ****** ****** ****** ****** ****** \*/
        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="CleanXmlSerializer&lt;TRoot&gt;"/> class.
        /// </summary>
        protected CleanXmlSerializer(Type type)
        {
            _serializer = new XmlSerializer(type);
            _namespaces = new XmlSerializerNamespaces();
            _namespaces.Add("", "");
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
        protected string ToXml(object source)
        {
            using (var writer = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    _serializer.Serialize(xmlWriter, source, _namespaces);

                    return writer.ToString();
                }
            }
        }

        /// <summary>
        /// Serializes the <paramref name="source"/> to XML.
        /// </summary>
        /// 
        /// <typeparam name="T">The type of the object that is to be serialized.</typeparam>
        /// <param name="source">The source.</param>
        /// 
        /// <returns>
        /// The <paramref name="source"/> serialized to XML.
        /// </returns>
        public static string ToXml<T>(T source)
        {
            var serializer = new CleanXmlSerializer<T>();

            return serializer.ToXml(source);
        }
    }
}
