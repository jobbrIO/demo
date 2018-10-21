using System;
using System.Diagnostics;
using Demo.MyJobs;
using Jobbr.Runtime;
using Jobbr.Runtime.ForkedExecution;
using Jobbr.Runtime.Logging;

namespace Demo.JobRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Redirect Log-Output to Trace, remove this if you install any other Log-Framework 
            LogProvider.SetCurrentLogProvider(new TraceLogProvider());

            // Make sure the compiler does not remove the binding to this assembly
            var jobAssemblyToQueryJobs = typeof(ProgressJob).Assembly;

            // Set the default assembly to query for jobtypes
            var runtime = new ForkedRuntime(new RuntimeConfiguration { JobTypeSearchAssemblies = new[] { jobAssemblyToQueryJobs } });

            // Pass the arguments of the forked execution to the runtime
            runtime.Run(args);
        }
    }

    public class TraceLogProvider : ILogProvider //, ForkedLogging.ILogProvider
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
