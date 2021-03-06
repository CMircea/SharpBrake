using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Common.Logging;
using SharpBrake.Serialization;

namespace SharpBrake
{
    /// <summary>
    /// Responsible for building the notice that is sent to Airbrake.
    /// </summary>
    public sealed class AirbrakeNoticeBuilder
    {
        private readonly ILog _log;
        private readonly AirbrakeConfiguration _configuration;

        private AirbrakeNotifier _notifier;
        private AirbrakeServerEnvironment _environment;

        /*\ ****** ****** ****** ****** ****** Properties ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public AirbrakeConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        /// <summary>
        /// Gets the notifier.
        /// </summary>
        public AirbrakeNotifier Notifier
        {
            get
            {
                if (_notifier != null)
                {
                    return _notifier;
                }
                else
                {
                    return _notifier = new AirbrakeNotifier
                    {
                        Name    = "SharpBrake",
                        Url     = "https://github.com/airbrake/SharpBrake",
                        Version = typeof(AirbrakeNotifier).Assembly.GetName().Version.ToString(),
                    };
                }
            }
        }

        /// <summary>
        /// Gets the server environment.
        /// </summary>
        public AirbrakeServerEnvironment ServerEnvironment
        {
            get
            {
                if (_environment != null)
                {
                    return _environment;
                }
                else
                {
                    return _environment = new AirbrakeServerEnvironment(_configuration.EnvironmentName)
                    {
                        ProjectRoot = _configuration.ProjectRoot,
                        AppVersion  = _configuration.AppVersion,
                    };
                }
            }
        }

        /*\ ****** ****** ****** ****** ****** Constructors ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeNoticeBuilder"/> class.
        /// </summary>
        public AirbrakeNoticeBuilder()
            : this(new AirbrakeConfiguration())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AirbrakeNoticeBuilder"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public AirbrakeNoticeBuilder(AirbrakeConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            _log           = LogManager.GetLogger(GetType());
            _configuration = configuration;
        }

        /*\ ****** ****** ****** ****** ****** Public Methods ****** ****** ****** ****** ****** \*/
        /// <summary>
        /// Creates a <see cref="AirbrakeError"/> from the the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A <see cref="AirbrakeError"/>, created from the the specified exception.
        /// </returns>
        public AirbrakeError ErrorFromException(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            _log.Debug(f => f("{0}.Notice({1})", GetType(), exception.GetType()), exception);

            MethodBase catchingMethod;
            var backtrace = BuildBacktrace(exception, out catchingMethod);

            return new AirbrakeError
            {
                CatchingMethod = catchingMethod,
                Class          = exception.GetType().FullName,
                Message        = exception.GetType().Name + ": " + exception.Message,
                Backtrace      = backtrace
            };
        }


        /// <summary>
        /// Creates a <see cref="AirbrakeNotice"/> from the the specified error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public AirbrakeNotice BuildNotice(AirbrakeError error)
        {
            _log.Debug(f => f("{0}.Notice({1})", GetType(), error));

            var notice = new AirbrakeNotice
            {
                ApiKey            = Configuration.ApiKey,
                Error             = error,
                Notifier          = Notifier,
                ServerEnvironment = ServerEnvironment,
            };

            MethodBase catchingMethod = error != null ? error.CatchingMethod : null;

            AddContextualInformation(notice, catchingMethod);

            return notice;
        }


        /// <summary>
        /// Creates a <see cref="AirbrakeNotice"/> from the the specified exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// A <see cref="AirbrakeNotice"/>, created from the the specified exception.
        /// </returns>
        public AirbrakeNotice BuildNotice(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            _log.Info(f => f("{0}.Notice({1})", GetType(), exception.GetType()), exception);

            return BuildNotice(ErrorFromException(exception));
        }

        /*\ ****** ****** ****** ****** ****** Private Methods ****** ****** ****** ****** ****** \*/
        private void AddContextualInformation(AirbrakeNotice notice, MethodBase catchingMethod)
        {
            var component = String.Empty;
            var action    = String.Empty;

            if (notice.Error != null && notice.Error.Backtrace != null && notice.Error.Backtrace.Any())
            {
                // TODO: We should perhaps check whether the topmost back trace is in fact a Controller+Action by performing some sort of heuristic (searching for "Controller" etc.). @asbjornu
                var backtrace = notice.Error.Backtrace.First();

                action    = backtrace.Method;
                component = backtrace.File;
            }
            else if (catchingMethod != null)
            {
                action = catchingMethod.Name;

                if (catchingMethod.DeclaringType != null)
                    component = catchingMethod.DeclaringType.FullName;
            }

            var request = new AirbrakeRequest("http://example.com/", component)
            {
                Action = action
            };

            var cgiData = new List<AirbrakeVar>
            {
                new AirbrakeVar("Environment.MachineName", Environment.MachineName),
                new AirbrakeVar("Environment.OSversion",   Environment.OSVersion),
                new AirbrakeVar("Environment.Version",     Environment.Version)
            };

            var parameters  = new List<AirbrakeVar>();
            var session     = new List<AirbrakeVar>();
            var httpContext = HttpContext.Current;

            if (httpContext != null)
            {
                var httpRequest = httpContext.Request;

                request.Url = httpRequest.Url.ToString();

                cgiData.AddRange(BuildVars(httpRequest.Headers));
                cgiData.AddRange(BuildVars(httpRequest.Cookies));
                session.AddRange(BuildVars(httpContext.Session));
                parameters.AddRange(BuildVars(httpRequest.Params));

                if (httpContext.User != null)
                    cgiData.Add(new AirbrakeVar("User.Identity.Name", httpContext.User.Identity.Name));

                var browser = httpRequest.Browser;

                if (browser != null)
                {
                    cgiData.Add(new AirbrakeVar("Browser.Browser", browser.Browser));
                    cgiData.Add(new AirbrakeVar("Browser.ClrVersion", browser.ClrVersion));
                    cgiData.Add(new AirbrakeVar("Browser.Cookies", browser.Cookies));
                    cgiData.Add(new AirbrakeVar("Browser.Crawler", browser.Crawler));
                    cgiData.Add(new AirbrakeVar("Browser.EcmaScriptVersion", browser.EcmaScriptVersion));
                    cgiData.Add(new AirbrakeVar("Browser.JavaApplets", browser.JavaApplets));
                    cgiData.Add(new AirbrakeVar("Browser.MajorVersion", browser.MajorVersion));
                    cgiData.Add(new AirbrakeVar("Browser.MinorVersion", browser.MinorVersion));
                    cgiData.Add(new AirbrakeVar("Browser.Platform", browser.Platform));
                    cgiData.Add(new AirbrakeVar("Browser.W3CDomVersion", browser.W3CDomVersion));
                }
            }

            request.CgiData = cgiData.ToArray();
            request.Params  = parameters.Any() ? parameters.ToArray() : null;
            request.Session = session.Any() ? session.ToArray() : null;

            notice.Request = request;
        }


        private AirbrakeTraceLine[] BuildBacktrace(Exception exception, out MethodBase catchingMethod)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            if (assembly.EntryPoint == null)
                assembly = Assembly.GetCallingAssembly();

            if (assembly.EntryPoint == null)
                assembly = Assembly.GetEntryAssembly();

            catchingMethod = assembly == null
                                 ? null
                                 : assembly.EntryPoint;

            var lines      = new List<AirbrakeTraceLine>();
            var stackTrace = new StackTrace(exception);
            var frames     = stackTrace.GetFrames();

            if (frames == null || frames.Length == 0)
            {
                // Airbrake requires that at least one line is present in the XML.
                lines.Add(new AirbrakeTraceLine("none", 0));

                return lines.ToArray();
            }

            foreach (StackFrame frame in frames)
            {
                MethodBase method = frame.GetMethod();

                catchingMethod = method;

                int lineNumber = frame.GetFileLineNumber();

                if (lineNumber == 0)
                {
                    _log.Debug(f => f("No line number found in {0}, using IL offset instead.", method));

                    lineNumber = frame.GetILOffset();
                }

                string file = frame.GetFileName();

                if (String.IsNullOrEmpty(file))
                    file = method.ReflectedType != null ? method.ReflectedType.FullName : "(unknown)";

                var line = new AirbrakeTraceLine(file, lineNumber)
                {
                    Method = method.Name
                };

                lines.Add(line);
            }

            return lines.ToArray();
        }

        private IEnumerable<AirbrakeVar> BuildVars(HttpCookieCollection cookies)
        {
            if ((cookies == null) || (cookies.Count == 0))
            {
                _log.Debug(f => f("No cookies to build vars from."));

                return new AirbrakeVar[0];
            }

            return from key in cookies.Keys.Cast<string>()
                   where !String.IsNullOrEmpty(key)
                   let cookie = cookies[key]
                   let value = cookie != null ? cookie.Value : null
                   where !String.IsNullOrEmpty(value)
                   select new AirbrakeVar(key, value);
        }

        private IEnumerable<AirbrakeVar> BuildVars(NameValueCollection formData)
        {
            if (formData == null || formData.Count == 0)
            {
                _log.Debug(f => f("No form data to build vars from."));

                return new AirbrakeVar[0];
            }

            return from key in formData.AllKeys
                   where !String.IsNullOrEmpty(key)
                   let value = formData[key]
                   where !String.IsNullOrEmpty(value)
                   select new AirbrakeVar(key, value);
        }

        private IEnumerable<AirbrakeVar> BuildVars(HttpSessionState session)
        {
            if (session == null || session.Count == 0)
            {
                _log.Debug(f => f("No session to build vars from."));

                return new AirbrakeVar[0];
            }

            return from key in session.Keys.Cast<string>()
                   where !String.IsNullOrEmpty(key)
                   let v = session[key]
                   let value = v != null ? v.ToString() : null
                   where !String.IsNullOrEmpty(value)
                   select new AirbrakeVar(key, value);
        }
    }
}
