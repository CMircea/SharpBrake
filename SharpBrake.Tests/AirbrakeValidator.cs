using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;

namespace SharpBrake.Tests
{
    public static class AirbrakeValidator
    {
        /*\ ****** ****** ****** ****** ****** Public Methods ****** ****** ****** ****** ****** \*/
        public static void ValidateSchema(string xml)
        {
            var schema = GetXmlSchema();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
            };

            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            settings.Schemas.Add(schema);

            using (var reader    = new StringReader(xml))
            using (var xmlReader = new XmlTextReader(reader))
            using (var validator = XmlReader.Create(xmlReader, settings))
            {
                while (validator.Read())
                {
                    // do nothing
                }
            }
        }

        /*\ ****** ****** ****** ****** ****** Private Methods ****** ****** ****** ****** ****** \*/
        private static XmlSchema GetXmlSchema()
        {
            const string xsd = "airbrake_2_2.xsd";

            Type clientType = typeof(AirbrakeClient);

            using (Stream schemaStream = clientType.Assembly.GetManifestResourceStream(clientType, xsd))
            {
                if (schemaStream == null)
                    Assert.Fail("{0}.{1} not found.", clientType.Namespace, xsd);

                return XmlSchema.Read(schemaStream, null);
            }
        }
    }
}
