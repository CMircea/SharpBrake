using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using SharpBrake.Serialization;

namespace SharpBrake.Tests
{
    [TestFixture]
    public sealed class CleanXmlGeneration
    {
        [XmlRoot("notice", Namespace = "")]
        public sealed class TestNotice
        {
            [XmlElement("api-key")]
            public string ApiKey { get; set; }

            [XmlAttribute("version")]
            public string Version { get; set; }
        }

        [Test]
        public void XmlContainsNoFluff()
        {
            var notice = new TestNotice
            {
                ApiKey  = "123456",
                Version = "2.0"
            };

            var serializer = new CleanXmlSerializer<TestNotice>();
            var xml        = serializer.ToXml(notice);

            Assert.AreEqual(xml, "<notice version=\"2.0\"><api-key>123456</api-key></notice>");
        }
    }
}
