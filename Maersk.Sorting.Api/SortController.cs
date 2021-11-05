using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("sort")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;

        public SortController(ISortJobProcessor sortJobProcessor)
        {
            _sortJobProcessor = sortJobProcessor;
        }

        [HttpPost("run")]
        [Obsolete("This executes the sort job asynchronously. Use the asynchronous 'EnqueueJob' instead.")]
        public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        {
            var pendingJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);

            var completedJob = await _sortJobProcessor.Process(pendingJob);

            return Ok(completedJob);
        }

        [HttpPost]
        public async Task<ActionResult<SortJob>> EnqueueJob(int[] values)
        {
            var pendingJob = new SortJob(
            id: Guid.NewGuid(),
            status: SortJobStatus.Pending,
            duration: null,
            input: values,
            output: null);
            //Client doesn't need to wait for the result in this case.
            //The Task.Factory.StartNew will invoke the Process method and returns
            await Task.Factory.StartNew(() => _sortJobProcessor.Process(pendingJob));
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<SortJob[]>> GetJobs()
        {
            var jobs = await _sortJobProcessor.GetJobs();
            return Ok(jobs);
        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<SortJob>> GetJob(Guid jobId)
        {
            var job = await _sortJobProcessor.GetJob(jobId);
            return Ok(job);
        }
    }
}
