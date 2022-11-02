using System;

namespace Demo.MyJobs
{
    public class ParameterizedJob
    {
        public void Run(object jobParameters, RunParameter runParameters)
        {
            Console.WriteLine("Got the params {0} and {1}", jobParameters, runParameters);
        }
    }

    public class RunParameter
    {
        public string Param1 { get; set; }

        public int Param2 { get; set; }
    }
}
