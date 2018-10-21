using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;

namespace Demo.JobServer
{
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