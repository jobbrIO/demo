using System.IO;

namespace Demo.MyJobs
{
    public class JobWithArtefacts
    {
        public void Run()
        {
            File.AppendAllText("random-artefact.txt", "lorem ipsum");
        }
    }
}