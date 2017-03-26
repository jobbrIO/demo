using System;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server;
using Jobbr.Server.Builder;
using Jobbr.Server.ForkedExecution;
using Jobbr.Server.JobRegistry;
using Jobbr.Server.MsSql;
using Jobbr.Server.WebAPI;

namespace Demo.JobServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var jobbrBuilder = new JobbrBuilder();

            // Dispatch the execution to a separate process. See the Project "Demo.JobRunner" for details
            jobbrBuilder.AddForkedExecution(config =>
                {
                    config.JobRunDirectory = "C:/temp";
                    config.JobRunnerExeResolver = () => "../../../Demo.JobRunner/bin/Debug/Demo.JobRunner.exe";
                    config.MaxConcurrentJobs = 1;
                    config.IsRuntimeWaitingForDebugger = true;
                }
            );

            // Setup an initial set of jobs with a unique name and the corresponding CLR Type.
            // Note: The Server does not reference the assembly containing the type since the Runner (see above) will activate and execute the job
            jobbrBuilder.AddJobs(repo =>
            {
                repo.Define("ProgressJob", "Demo.MyJobs.ProgressJob")
                    .WithTrigger("* * * * *");
            });

            // Expose a simple Rest-API that is compatible with any browser and the Jobbr.Client
            jobbrBuilder.AddWebApi(config =>
            {
                config.BackendAddress = "http://localhost:1337";
            });

            // Uncomment to use sql server as storage
            //jobbrBuilder.AddMsSqlStorage(c =>
            //{
            //    c.ConnectionString = "connectionstring";
            //    c.Schema = "Jobbr";
            //});

            // Register your very own component that gets as JobbrComponent and can request specific implementations with constructor injection
            jobbrBuilder.Register<IJobbrComponent>(typeof(MyExtension));

            using (var server = jobbrBuilder.Create())
            {
                server.Start();

                // Trigger a new Job from here. How-ever this does not make sense usually... 
                // Better approach would be to use the Client Libraries to access the WebAPI
                // MyExtension.Instance.JobManagementService.AddTrigger(new InstantTrigger() { JobId = 1, IsActive = true });

                Console.ReadLine();

                server.Stop();
            }
        }
    }

    public class MyExtension : IJobbrComponent
    {
        public static MyExtension Instance;

        private readonly IJobbrServiceProvider serviceProvider;
        private readonly IJobManagementService jobManagementService;
        private readonly IQueryService queryService;


        public IJobManagementService JobManagementService => jobManagementService;

        public MyExtension(IJobbrServiceProvider serviceProvider, IJobManagementService jobManagementService, IQueryService queryService)
        {
            Instance = this;

            // you can request basically anything that is available in the JobbrDI. The list of available types are 
            // defined in the corresponding component models available on http://github.com/jobbrIO
            this.serviceProvider = serviceProvider;
            this.jobManagementService = jobManagementService;
            this.queryService = queryService;
        }

        public void Dispose() { }

        public void Start() { }

        public void Stop() { }
    }
}
