using System;
using System.Diagnostics;
using System.IO;
using Jobbr.ArtefactStorage.FileSystem;
using Jobbr.ComponentModel.Registration;
// using Jobbr.Dashboard;
using Jobbr.Server.Builder;
using Jobbr.Server.ForkedExecution;
using Jobbr.Server.JobRegistry;
using Jobbr.Server.WebAPI;
using Jobbr.Storage.MsSql;

namespace Demo.JobServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const string baseUrl = "http://localhost:1338/";
            const string jobRunDirectory = "C:/temp";

            if (Directory.Exists(jobRunDirectory) == false)
            {
                Directory.CreateDirectory(jobRunDirectory);
            }

            var jobbrBuilder = new JobbrBuilder();

            jobbrBuilder.AddFileSystemArtefactStorage(config =>
            {
                config.DataDirectory = Directory.GetCurrentDirectory();
            });

            // Dispatch the execution to a separate process. See the Project "Demo.JobRunner" for details
            jobbrBuilder.AddForkedExecution(config =>
                {
                    config.JobRunDirectory = jobRunDirectory;
                    config.JobRunnerExecutable = "../../../Demo.JobRunner/bin/Debug/Demo.JobRunner.exe";
                    config.MaxConcurrentProcesses = 1;
                    config.IsRuntimeWaitingForDebugger = false;
                }
            );

            // Setup an initial set of jobs with a unique name and the corresponding CLR Type.
            // Note: The Server does not reference the assembly containing the type since the Runner (see above) will activate and execute the job
            jobbrBuilder.AddJobs(repo =>
            {
                repo.Define("ProgressJob", "Demo.MyJobs.ProgressJob")
                    .WithTrigger("* * * * *");

                repo.Define("ArtefactJob", "Demo.MyJobs.JobWithArtefacts")
                    .WithTrigger("* * * * *");
            });

            // Expose a Rest-API that is compatible with any browser and the Jobbr.Client
            jobbrBuilder.AddWebApi(config =>
            {
                config.BackendAddress = $"{baseUrl}api";
            });

            // Choose one of the following two storage providers (MsSQL or RavenDB). The Jobbr server will use
            // its own in memory storage provider, if none of the two is configured. The in memory storage provider should only
            // be used for unit testing or getting started with Jobbr.

            // Uncomment to use sql server as storage
            jobbrBuilder.AddMsSqlStorage(c =>
            {
                c.ConnectionString = @"Data Source=localhost\MSSQLSERVER01;Initial Catalog=JobbrDemo;Connect Timeout=10;Integrated Security=True";
                c.CreateTablesIfNotExists = true;
            });

            // TODO: RavenDB Storage does not implement JobStorage cm v1.3 -> can not be used atm :(
            // Uncomment to use RavenDB as storage
            // (start a ravendb server by executing packages\RavenDB.Server.3.5.3\tools\RavenDB.Server.exe)
            //jobbrBuilder.AddRavenDbStorage(config =>
            //{
            //    config.Url = "http://localhost:8080";
            //    config.Database = "Jobbr";
            //});

            //jobbrBuilder.AddDashboard(config =>
            //{
            //    config.BackendAddress = $"{baseUrl}";
            //    config.SoftDeleteJobRunOnRetry = true;
            //});

            // Register your very own component that gets as JobbrComponent and can request specific implementations with constructor injection
            jobbrBuilder.Register<IJobbrComponent>(typeof(MyExtension));

            using (var server = jobbrBuilder.Create())
            {
                server.Start(20000);

                Process.Start(baseUrl);

                // Trigger a new Job from here. How-ever this does not make sense usually... 
                // Better approach would be to use the Client Libraries to access the WebAPI
                // MyExtension.Instance.JobManagementService.AddTrigger(new InstantTrigger() { JobId = 1, IsActive = true });

                Console.ReadLine();

                server.Stop();
            }
        }
    }
}
