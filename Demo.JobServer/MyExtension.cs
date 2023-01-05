using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;

namespace Demo.JobServer
{
    public class MyExtension : IJobbrComponent
    {
        public static MyExtension Instance;

        private readonly IJobbrServiceProvider _serviceProvider;
        private readonly IJobManagementService _jobManagementService;
        private readonly IQueryService _queryService;

        public IJobManagementService JobManagementService => _jobManagementService;

        public MyExtension(IJobbrServiceProvider serviceProvider, IJobManagementService jobManagementService, IQueryService queryService)
        {
            Instance = this;

            // you can request basically anything that is available in the JobbrDI. The list of available types are 
            // defined in the corresponding component models available on http://github.com/jobbrIO
            _serviceProvider = serviceProvider;
            _jobManagementService = jobManagementService;
            _queryService = queryService;
        }

        public void Dispose() { }

        public void Start() { }

        public void Stop() { }
    }
}