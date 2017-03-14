using System;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.Server;
using Jobbr.Server.Builder;
using Jobbr.Server.ForkedExecution;
using Jobbr.Server.JobRegistry;
using Jobbr.Server.MsSql;

namespace Demo.JobServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var jobbrBuilder = new JobbrBuilder();
            jobbrBuilder.AddForkedExecution(config =>
                {
                    config.JobRunDirectory = "C:/temp";
                    config.JobRunnerExeResolver = () => "../../../Demo.JobRunner/bin/Debug/Demo.JobRunner.exe";
                    config.MaxConcurrentJobs = 1;
                    config.IsRuntimeWaitingForDebugger = true;
                }
            );

            jobbrBuilder.AddJobs(repo =>
            {
                repo.Define("ProgressJob", "Demo.MyJobs.ProgressJob")
                    .WithTrigger("* * * * *");
            });

            // Uncomment to use sql server as storage

            //jobbrBuilder.AddMsSqlStorage(c =>
            //{
            //    c.ConnectionString = "connectionstring";
            //    c.Schema = "Jobbr";
            //});

            using (var server = jobbrBuilder.Create())
            {
                server.Start();
                
                Console.ReadLine();

                server.Stop();
            }
        }
    }
}
