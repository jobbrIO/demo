# Jobbr Demo [![AppVeyor branch](https://img.shields.io/appveyor/ci/jobbr/jobbr-demo/develop.svg)]()
This repository contains a sample setup of Jobbr to get you started.

# Quick Start
Clone this repo and run the solution. The following steps are already prepared in the demo solution.

## Jobbr Server
Install Jobbr Server NuGet and the forked execution model.
```
Install-Package Jobbr.Server
Install-Package Jobbr.Execution.Forked
```

Configure Jobbr Server in code
```c#
var jobbrBuilder = new JobbrBuilder();

// tell jobbr to start seperate processes for running jobs
jobbrBuilder.AddForkedExecution(config =>
    {
        config.JobRunDirectory = "C:/temp";
        config.JobRunnerExecutable = "./JobRunner.exe";
        config.MaxConcurrentProcesses = 1;
    }
);
```

The Jobbr Server can be hosted as a console application, windows service or embedded in your application.
```c#
using (var server = jobbrBuilder.Create())
{
    server.Start();

    Console.ReadLine();

    server.Stop();
}
```

Jobs and triggers can be defined in code, where as the "TestJob" can be considered as unique name of your job and the "JobRunnerSample.TestJob" is the full qualified name of the class-type that contains your Run()-Method as the entry point to your code:

```c#
jobbrBuilder.AddJobs(repo =>
{
    // define one job with two triggers
    repo.Define("TestJob", "JobRunnerSample.TestJob")
        .WithTrigger(DateTime.UtcNow.AddHours(2)) /* run once in two hours */
        .WithTrigger("* * * * *"); /* run every minute */
});
```
## JobRunner
In order to run jobs create a Console application project JobRunner which compiles into the JobRunner.exe executable configured in the ForkedExecution configuration above.

```
Install-Package Jobbr.Runtime.ForkedExecution
```

```c#
namespace JobRunnerSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // start the jobbr runtime and tell it where to find
            // the defined job types.
            var runtime = new ForkedRuntime(
                new RuntimeConfiguration { JobTypeSearchAssembly = typeof(Program).Assembly) }
            );

            // Pass the arguments of the forked execution to
            // the runtime which executes the Run method of 
            // the configured job
            runtime.Run(args);
        }
    }

    public class TestJob
    {
        public void Run()
        {
            // do something
        }
    }
}
```

Note that there is no reference to the jobserver project from the jobrunner or vice versa. Only the runtime (JobRunner.exe) knows how to instantiate job classes. All the information the runtime needs to execute the jobs are passed via args.

## Storage
If no `IJobStorageProvider` is configured, jobs/triggers/jobruns are persisted in memory and lost when the server is restarted. In memory storage is only suitable for testing and getting started with jobbr. Choose one of the available storage providers to persist the data:

### MsSQL storage
```
Install-Package Jobbr.Server.MsSql
```

configure jobbr to use MsSql storage before creating the server instance:
```c#
jobbrBuilder.AddMsSqlStorage(c =>
{
    c.ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\temp\jobbr.mdf;Integrated Security=True;Connect Timeout=30";
    c.Schema = "Jobbr";
});
```
Source code: [JobbrIO/jobbr-server-mssql](https://github.com/jobbrIO/jobbr-storage-mssql)

### RavenDB storage
```
Install-Package Jobbr.Server.RavenDB
```
```c#
jobbrBuilder.AddRavenDbStorage(config =>
{
    config.Url = "http://localhost:8080";
    config.Database = "Jobbr";
});
```
Source code: [JobbrIO/jobbr-server-ravendb](https://github.com/jobbrIO/jobbr-storage-ravendb)

## Artefacts
Artefacts are files which jobs write to their working directory during execution. These files are collected when the job finished and persisted by the jobbr server. Like the `JobStorageProvider` if you don't configure an `IArtefactStorageProvider`, these artefacts are only persisted to memory and lost after restart. To persist them choose one of the following artefact storage providers:

- [JobbrIO/jobbr-artefactstorage-filesystem](https://github.com/jobbrIO/jobbr-artefactstorage-filesystem)
- [JobbrIO/jobbr-artefactstorage-ravenfs](https://github.com/jobbrIO/jobbr-artefactstorage-ravenfs)

## WebAPI
To interact (eg creating new triggers) with jobbr server at runtime, add the [WebAPI](https://github.com/jobbrIO/jobbr-webapi) component to your server configuration. This component will expose a REST API to interact with the server at runtime.

```
Install-Package Jobbr.Server.WebAPI
```

```c#
builder.AddWebApi(config => 
{
	config.BaseUrl = "http://localhost:1337/jobbr";
});
```

### Client
Use the typed [Client](https://www.nuget.org/packages/Jobbr.Client) to access the WebAPI:

```c#
var jobbrClient = new JobbrClient("http://localhost:1337/jobbr");

var allJobs = jobbrClient.GetAllJobs();

var createdTrigger = jobbrClient.AddTrigger(jobId, new ScheduledTriggerDto { IsActive = true, StartDateTimeUtc = DateTime.UtcNow.AddMinutes(30) });
```
