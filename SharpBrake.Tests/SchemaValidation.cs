using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using NUnit.Framework;
using SharpBrake.Serialization;

namespace SharpBrake.Tests
{
    [TestFixture]
    public sealed class SchemaValidation
    {
        [Test]
        public void MaximalNoticeGeneratesValidXml()
        {
            var error  = new AirbrakeError
            {
                Class   = "TestError",
                Message = "something blew up",

                Backtrace = new[]
                {
                    new AirbrakeTraceLine("unknown.cs", 0)
                    {
                        Method = "unknown"
                    }
                }
            };

            var notice = new AirbrakeNotice
            {
                ApiKey = "123456",
                Error  = error,

                Request = new AirbrakeRequest(new Uri("http://example.com/"), GetType().FullName)
                {
                    Action    = "MaximalNoticeGeneratesValidXml",
                    Component = "MyApp.HomeController",
                    Url       = "http://example.com/myapp",

                    CgiData = new[]
                    {
                        new AirbrakeVar("REQUEST_METHOD", "POST"),
                    },

                    Params = new[]
                    {
                        new AirbrakeVar("Form.Key1", "Form.Value1"),
                    },

                    Session = new[]
                    {
                        new AirbrakeVar("UserId", "1"),
                    },
                },

                Notifier = new AirbrakeNotifier
                {
                    Name    = "sharpbrake",
                    Version = "2.2.2.0",
                    Url     = "https://github.com/airbrake/SharpBrake",
                },

                ServerEnvironment = new AirbrakeServerEnvironment("staging")
                {
                    ProjectRoot = "/test",
                },
            };

            var serializer = new CleanXmlSerializer<AirbrakeNotice>();
            var xml        = serializer.ToXml(notice);

            AirbrakeValidator.ValidateSchema(xml);
        }

        [Test]
        public void MinimalNoticeGeneratesValidXml()
        {
            var error = new AirbrakeError
            {
                Class   = "TestError",
                Message = "something blew up",

                Backtrace = new[]
                {
                    new AirbrakeTraceLine("unknown.cs", 0)
                    {
                        Method = "unknown"
                    }
                }
            };

            var notice = new AirbrakeNotice
            {
                ApiKey = "123456",
                Error  = error,

                Notifier = new AirbrakeNotifier
                {
                    Name    = "sharpbrake",
                    Version = "2.2.2.0",
                    Url     = "https://github.com/airbrake/SharpBrake"
                },

                ServerEnvironment = new AirbrakeServerEnvironment("staging")
                {
                    ProjectRoot = "/test",
                },
            };

            var serializer = new CleanXmlSerializer<AirbrakeNotice>();
            var xml        = serializer.ToXml(notice);

            AirbrakeValidator.ValidateSchema(xml);
        }

        [Test]
        public void MinimalNoticeWithRequestGeneratesValidXml()
        {
            var error = new AirbrakeError
            {
                Class   = "TestError",
                Message = "something blew up",

                Backtrace = new[]
                {
                    new AirbrakeTraceLine("unknown.cs", 0)
                    {
                        Method = "unknown"
                    }
                }
            };

            var notice = new AirbrakeNotice
            {
                ApiKey = "123456",
                Error  = error,

                Request = new AirbrakeRequest(new Uri("http://example.com/"), GetType().FullName)
                {
                    Session = new AirbrakeVar[0]
                },

                Notifier = new AirbrakeNotifier
                {
                    Name    = "sharpbrake",
                    Version = "2.2.2.0",
                    Url     = "https://github.com/airbrake/SharpBrake"
                },

                ServerEnvironment = new AirbrakeServerEnvironment("staging")
                {
                    ProjectRoot = "/test",
                },
            };

            var serializer = new CleanXmlSerializer<AirbrakeNotice>();
            var xml        = serializer.ToXml(notice);

            AirbrakeValidator.ValidateSchema(xml);
        }

        [Test]
        public void NoticeMissingErrorFailsValidation()
        {
            var notice = new AirbrakeNotice
            {
                ApiKey = "123456",

                Request = new AirbrakeRequest(new Uri("http://example.com/"), GetType().FullName)
                {
                    Action = "NoticeMissingErrorFailsValidation",
                },

                Notifier = new AirbrakeNotifier
                {
                    Name    = "sharpbrake",
                    Version = "2.2.2.0",
                    Url     = "https://github.com/airbrake/SharpBrake"
                },

                ServerEnvironment = new AirbrakeServerEnvironment("staging")
                {
                    ProjectRoot = "/test",
                },
            };

            var serializer = new CleanXmlSerializer<AirbrakeNotice>();
            var xml       = serializer.ToXml(notice);

            TestDelegate throwing = () => AirbrakeValidator.ValidateSchema(xml);

            var exception = Assert.Throws<XmlSchemaValidationException>(throwing);

            Console.WriteLine(exception);

            Assert.That(exception.Message, Is.StringContaining("notice"));
            Assert.That(exception.Message, Is.StringContaining("error"));
        }
    }
}
