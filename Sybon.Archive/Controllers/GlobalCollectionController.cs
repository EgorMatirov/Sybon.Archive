using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Archive.Services.CollectionsService;
using Sybon.Archive.Services.CollectionsService.Models;
using Sybon.Archive.Services.GlobalCollectionService;
using Sybon.Archive.Services.GlobalCollectionService.Models;
using Sybon.Archive.Services.ProblemsService;

namespace Sybon.Archive.Controllers
{
    [Route("api/[controller]")]
    public class GlobalCollectionController : Controller, ILogged
    {
        [HttpGet]
        [SwaggerOperation("Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ICollection<ProblemCollectionWithoutProblems>))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> Get([FromServices] IGlobalCollectionService globalCollectionService)
        {
            var collection = await globalCollectionService.GetAllAsync();
            foreach (var problem in collection)
                problem.InternalProblemId = string.Empty;
            return Ok(collection);
        }
        
        [HttpPost("problems")]
        [SwaggerOperation("AddProblem")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter(true)]
        public async Task<IActionResult> AddProblem([FromServices] IGlobalCollectionService globalCollectionService, [FromQuery] string internalProblemId)
        {
            var problemId = await globalCollectionService.AddAsync(internalProblemId);
            return Ok(problemId);
        }
        
        [HttpDelete("problems")]
        [SwaggerOperation("DeleteProblems")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter(true)]
        public async Task<IActionResult> DeleteProblems([FromServices] IGlobalCollectionService globalCollectionService, [FromBody] long[] problemIds)
        {
            await globalCollectionService.RemoveRangeAsync(problemIds);
            return Ok();
        }
        
        [HttpGet("problems/{id}")]
        [SwaggerOperation("GetById")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(GlobalCollectionProblem))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> Get([FromServices] IGlobalCollectionService globalCollectionService, long id)
        {
            var problem = await globalCollectionService.GetProblemAsync(id);
            return Ok(problem);
        }

        public long UserId { get; set; }
        public string ApiKey { get; set; }
    }
}