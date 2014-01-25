using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;
using SharpBrake.Serialization;
using Subtext.TestLibrary;

namespace SharpBrake.Tests
{
    [TestFixture]
    public sealed class NoticeComponentsCreation
    {
        private AirbrakeConfiguration _config;
        private AirbrakeNoticeBuilder _builder;

        /*\ ****** ****** ****** ****** ****** Setup ****** ****** ****** ****** ****** \*/
        [SetUp]
        public void SetUp()
        {
            _config = new AirbrakeConfiguration
            {
                ApiKey      = "123456",
                EnvironmentName = "test"
            };

            _builder = new AirbrakeNoticeBuilder(_config);
        }

        [Test]
        public void BuildingErrorFromDotNetException()
        {
            Exception exception;

            try
            {
                throw new InvalidOperationException("test error");
            }
            catch (Exception testException)
            {
                exception = testException;
            }

            var error = _builder.ErrorFromException(exception);

            Assert.IsNotEmpty(error.Backtrace);

            var trace = error.Backtrace[0];

            Assert.AreEqual(trace.Method, "BuildingErrorFromDotNetException");
            Assert.Greater(trace.LineNumber, 0);
        }

        [Test]
        public void NoticeContainsRequest()
        {
            AirbrakeNotice notice = null;

            const string url     = "http://example.com/?Query.Key1=Query.Value1&Query.Key2=Query.Value2";
            const string referer = "http://github.com/";

            string physicalApplicationPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar;

            var httpSimulator = new HttpSimulator("/", physicalApplicationPath)
                .SetFormVariable("Form.Key1", "Form.Value1")
                .SetFormVariable("Form.Key2", "Form.Value2")
                .SetHeader("Header.Key1", "Header.Value1")
                .SetHeader("Header.Key2", "Header.Value2")
                .SetReferer(new Uri(referer))
                .SimulateRequest(new Uri(url));

            using (httpSimulator)
            {
                try
                {
                    Thrower.Throw(new Exception("Halp!"));
                }
                catch (Exception exception)
                {
                    notice = _builder.BuildNotice(_builder.ErrorFromException(exception));
                }
            }

            Console.WriteLine(CleanXmlSerializer.ToXml(notice));

            Assert.IsNotNull(notice);
            Assert.IsNotNull(notice.Error);
            Assert.IsNotNull(notice.Request);

            Assert.AreEqual(notice.Request.Url, url);
            Assert.AreEqual(notice.Request.Component, (typeof(Thrower).FullName));
            Assert.AreEqual(notice.Request.Action, "Throw");

            Assert.That(notice.Request.CgiData, Contains.Item(new AirbrakeVar("Content-Type", "application/x-www-form-urlencoded")));
            Assert.That(notice.Request.CgiData, Contains.Item(new AirbrakeVar("Header.Key1", "Header.Value1")));
            Assert.That(notice.Request.CgiData, Contains.Item(new AirbrakeVar("Header.Key2", "Header.Value2")));
            Assert.That(notice.Request.CgiData, Contains.Item(new AirbrakeVar("Referer", referer)));

            Assert.That(notice.Request.Params, Contains.Item(new AirbrakeVar("APPL_PHYSICAL_PATH", physicalApplicationPath)));
            Assert.That(notice.Request.Params, Contains.Item(new AirbrakeVar("QUERY_STRING", "Query.Key1=Query.Value1&Query.Key2=Query.Value2")));
            Assert.That(notice.Request.Params, Contains.Item(new AirbrakeVar("Form.Key1", "Form.Value1")));
            Assert.That(notice.Request.Params, Contains.Item(new AirbrakeVar("Form.Key2", "Form.Value2")));
            Assert.That(notice.Request.Params, Contains.Item(new AirbrakeVar("Query.Key1", "Query.Value1")));
            Assert.That(notice.Request.Params, Contains.Item(new AirbrakeVar("Query.Key2", "Query.Value2")));
        }

        [Test]
        public void NoticeContainsServerEnvironmentAndNotifier()
        {
            var notice = _builder.BuildNotice((AirbrakeError) null);

            Assert.IsNotNull(notice.ServerEnvironment);
            Assert.IsNotNull(notice.ServerEnvironment.ProjectRoot);
            Assert.IsNotNull(notice.ServerEnvironment.EnvironmentName);

            Assert.IsNotNullOrEmpty(notice.ApiKey);
            Assert.IsNotNull(notice.Notifier);
            Assert.IsNotNull(notice.Version);
        }

        [Test]
        public void NotifierInitializedCorrectly()
        {
            AirbrakeNotifier notifier = _builder.Notifier;

            Assert.AreEqual(notifier.Name,    "SharpBrake");
            Assert.AreEqual(notifier.Url,     "https://github.com/airbrake/SharpBrake");
            Assert.AreEqual(notifier.Version, "2.2.2.0");
        }

        [Test]
        public void ServerEnvironmentReadFromAirbrakeConfig()
        {
            Assert.AreEqual(_builder.ServerEnvironment.EnvironmentName, _config.EnvironmentName);
        }

        [Test]
        public void StackTraceContainsLambdaExpression()
        {
            Exception exception = null;

            try
            {
                Expression<Func<int>> inner = () => ((string) null).Length;

                inner.Compile()();
            }
            catch (Exception testException)
            {
                exception = testException;
            }

            Assert.IsNotNull(_builder.ErrorFromException(exception));
        }
    }
}
