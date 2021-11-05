using System;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface ISortJobProcessor
    {
        Task<SortJob> Process(SortJob job);

        Task<SortJob[]> GetJobs();

        Task<SortJob?> GetJob(Guid jobId);
    }
}