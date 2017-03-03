using System.Reflection;
using Jobbr.Runtime.Console;

namespace Demo.JobRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var runtime = new JobbrRuntime(Assembly.GetEntryAssembly());

            runtime.Run(args);
        }
    }
}
