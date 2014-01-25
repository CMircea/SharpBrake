using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpBrake.Serialization;

namespace SharpBrake.Tests
{
    [TestFixture]
    public sealed class AirbrakeClientTests
    {
        private AirbrakeClient _client;

        /*\ ****** ****** ****** ****** ****** Setup ****** ****** ****** ****** ****** \*/
        [SetUp]
        public void SetUp()
        {
            _client = new AirbrakeClient();
        }

        /*\ ****** ****** ****** ****** ****** Test Methods ****** ****** ****** ****** ****** \*/
        [Test]
        [Ignore("This test needs to be rewritten for the 2.2 API")]
        public void SendEndRequestEventIsInvokedAndResponseOnlyContainsApiError()
        {
            bool requestEndInvoked = false;
            IList<AirbrakeResponseError> errors = null;

            _client.RequestEnd += (sender, e) =>
            {
                requestEndInvoked = true;

                errors = e.Response.Errors;
            };

            var configuration = new AirbrakeConfiguration
            {
                ApiKey      = Guid.NewGuid().ToString("N"),
                EnvironmentName = "test",
            };

            var builder = new AirbrakeNoticeBuilder(configuration);

            AirbrakeNotice notice = builder.BuildNotice(new Exception("Test"));

            notice.Request = new AirbrakeRequest("http://example.com", "Test")
            {
                Params = new[]
                {
                    new AirbrakeVar("TestKey", "TestValue")
                }
            };

            _client.Send(notice);

            Assert.That(requestEndInvoked, Is.True.After(5000));
            Assert.That(errors, Is.Not.Null);
            Assert.That(errors, Has.Length.EqualTo(1));
        }
    }
}
