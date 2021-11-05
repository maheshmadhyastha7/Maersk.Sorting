using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;

        private readonly ConcurrentDictionary<Guid, SortJob> _sortJobs;

        public SortJobProcessor(ILogger<SortJobProcessor> logger)
        {
            _logger = logger;
            _sortJobs = new ConcurrentDictionary<Guid, SortJob>();
        }

        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            //Try adding the Sort job if not added
            _sortJobs.TryAdd(job.Id, job);

            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(5000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            var sortedJob = new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output);

            //Update the status of the sorted job
            _sortJobs.TryUpdate(job.Id, sortedJob, job);

            return sortedJob;
        }

        /// <summary>
        /// Gets the specific job with id
        /// </summary>
        /// <param name="jobId">Job id</param>
        /// <returns></returns>
        public async Task<SortJob?> GetJob(Guid jobId) =>
             await Task.Run(async () => _sortJobs.TryGetValue(jobId, out SortJob? sortJob) == true ? sortJob :
                                                               await Task.FromResult<SortJob?>(null));

        /// <summary>
        /// Gets all the sorted jobs
        /// </summary>
        /// <returns></returns>
        public async Task<SortJob[]> GetJobs() =>
             await Task.Run(() => _sortJobs.Values.ToArray());



    }
}
