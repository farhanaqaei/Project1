using Project1.Core.Generals.Interfaces;

namespace Project1.Infrastructure.Jobs;

public class JobService: IJobService
{
    public void WriteJobExecutionTime()
    {
        Console.WriteLine($"Job executed at {DateTime.Now}");
    }
}
