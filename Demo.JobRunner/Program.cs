using System;
using System.Diagnostics;
using Demo.MyJobs;
using Jobbr.Runtime.Core;
using Jobbr.Runtime.ForkedExecution;
using CoreLogging = Jobbr.Runtime.Core.Logging;
using ForkedLogging = Jobbr.Runtime.ForkedExecution.Logging;

namespace Demo.JobRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Redirect Log-Output to Trace, remove this if you install any other Log-Framework 
            //ForkedLogging.LogProvider.SetCurrentLogProvider(new TraceLogProvider());
            CoreLogging.LogProvider.SetCurrentLogProvider(new TraceLogProvider());

            // Make sure the compiler does not remove the binding to this assembly
            var jobAssemblyToQueryJobs = typeof(ProgressJob).Assembly;

            // Set the default assembly to query for jobtypes
            var runtime = new ForkedRuntime(new RuntimeConfiguration { JobTypeSearchAssembly = jobAssemblyToQueryJobs });

            // Pass the arguments of the forked execution to the runtime
            runtime.Run(args);
        }
    }

    public class TraceLogProvider : CoreLogging.ILogProvider //, ForkedLogging.ILogProvider
    {
        public CoreLogging.ILog GetLogger(string name)
        {
            return (CoreLogging.ILog)new TraceLogger(name);
        }

        //public ForkedLogging.ILog ForkedLogging.GetLogger(string name)
        //{
        //    return (ForkedLogging.ILog)new TraceLogger(name);
        //}

        public IDisposable OpenNestedContext(string message)
        {
            return default(IDisposable);
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            return default(IDisposable);
        }
    }

    public class TraceLogger : CoreLogging.ILog
    {
        private readonly string _name;

        public TraceLogger(string name)
        {
            _name = name;
        }

        public bool Log(CoreLogging.LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
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
