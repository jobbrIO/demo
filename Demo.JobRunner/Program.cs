using System.Reflection;
using System.Diagnostics;
using Jobbr.ConsoleApp.Runtime.Logging;
using Jobbr.Runtime.Console;

namespace Demo.JobRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var runtime = new JobbrRuntime(Assembly.GetEntryAssembly());
            // Redirect Log-Output to Trace, remove this if you install any other Log-Framework 
            LogProvider.SetCurrentLogProvider(new TraceLogProvider());

            runtime.Run(args);
        }
    }

    public class TraceLogProvider : ILogProvider
    {
        public ILog GetLogger(string name)
        {
            return new TraceLogger(name);
        }

        public IDisposable OpenNestedContext(string message)
        {
            return default(IDisposable);
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            return default(IDisposable);
        }
    }

    public class TraceLogger : ILog
    {
        private readonly string _name;

        public TraceLogger(string name)
        {
            _name = name;
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
        {
            if (messageFunc == null)
            {
                return true;
            }

            Trace.WriteLine($"[{logLevel}] ({_name}) {string.Format(messageFunc(), formatParameters)} {exception}");
            return true;
        }
    }
}
