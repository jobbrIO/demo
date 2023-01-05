using Demo.MyJobs;
using Jobbr.Runtime;
using Jobbr.Runtime.ForkedExecution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Demo.JobRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = CreateHostBuilder(args).Build();
            var loggerFactory = app.Services.GetService<ILoggerFactory>();

            // Make sure the compiler does not remove the binding to this assembly
            var jobAssemblyToQueryJobs = typeof(ProgressJob).Assembly;

            // Set the default assembly to query for job types
            var runtime = new ForkedRuntime(loggerFactory, new RuntimeConfiguration { JobTypeSearchAssemblies = new[] { jobAssemblyToQueryJobs } });

            // Pass the arguments of the forked execution to the runtime
            runtime.Run(args);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
    }
}
