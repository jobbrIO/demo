using System;
using System.Linq;
using Jobbr.Client;
using Jobbr.Server.WebAPI.Model;

namespace Demo.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var jobbrClient = new JobbrClient("http://localhost:1338");

            var allJobs = jobbrClient.QueryJobs();

            if (allJobs.TotalItems == 0)
            {
                Console.WriteLine("At least one job is required to run this demo. Press enter to quit...");
                Console.ReadLine();
                return;
            }

            var jobId = allJobs.Items.First().Id;

            var trigger = jobbrClient.AddTrigger(jobId, new InstantTriggerDto { IsActive = true, UserDisplayName = "userName" });
            Console.WriteLine("Got Trigger with Id:" + trigger.Id);

            var jobRuns = jobbrClient.GetJobRunsByTriggerId(jobId, trigger.Id);
            Console.WriteLine("There are {0} jobruns assigned to this trigger id.", jobRuns.TotalItems);

            var jobRun = jobbrClient.GetJobRunById(jobRuns.Items[0].JobRunId);
            Console.WriteLine("Current State: " + jobRun.State);
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.ReadLine();

            var createdTrigger = jobbrClient.AddTrigger(jobId, new ScheduledTriggerDto { IsActive = true, UserDisplayName = "userName", StartDateTimeUtc = DateTime.UtcNow.AddMinutes(30) });
            Console.WriteLine("Created FutureTrigger with Id:" + trigger.Id + ", IsActive: " + createdTrigger.IsActive);

            var futureTrigger = jobbrClient.GetTriggerById<ScheduledTriggerDto>(jobId, createdTrigger.Id);
            Console.WriteLine("Got FutureTrigger by Id:" + trigger.Id + ", IsActive: " + createdTrigger.IsActive);

            var disableTriggerInfo = new ScheduledTriggerDto() { Id = futureTrigger.Id, IsActive = false };
            var deactivatedTrigger = jobbrClient.UpdateTrigger(jobId, disableTriggerInfo);

            Console.WriteLine("Disabled FutureTrigger width Id:" + trigger.Id + ", IsActive: " + deactivatedTrigger.IsActive);

            Console.WriteLine("Press enter to quit...");
            Console.ReadLine();
        }
    }
}
