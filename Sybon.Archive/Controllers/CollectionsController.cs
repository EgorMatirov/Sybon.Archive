using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Archive.Services.CollectionsService;
using Sybon.Archive.Services.CollectionsService.Models;
using Sybon.Archive.Services.ProblemsService;

namespace Sybon.Archive.Controllers
{
    [Route("api/[controller]")]
    public class CollectionsController : Controller, ILogged
    {
        [HttpGet]
        [SwaggerOperation("Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ICollection<ProblemCollection>))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> Get([FromServices] ICollectionsService collectionsService, [FromQuery] Pagination pagination)
        {
            var collections = await collectionsService.GetRangeAsync(pagination.Offset, pagination.Limit);
            return Ok(collections);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("GetById")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ProblemCollection))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        //[PermissionFilter("collection", "read", "id")] //TODO
        public async Task<IActionResult> Get([FromServices] ICollectionsService collectionsService, long id)
        {
            var collection = await collectionsService.FindAsync(id);
            foreach (var problem in collection.Problems)
                problem.InternalProblemId = string.Empty;
            return Ok(collection);
        }

        [HttpPost]
        [SwaggerOperation("Add")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter(true)]
        public async Task<IActionResult> Add([FromServices] ICollectionsService collectionsService, [FromBody] ProblemCollection collection)
        {
            var id = await collectionsService.AddAsync(UserId, collection);
            return Ok(id);
        }

        [HttpPost("{collectionId}/problems")]
        [SwaggerOperation("GetById")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        //[PermissionFilter("collection", "write", "collectionId")] //TODO
        public async Task<IActionResult> AddProblem([FromServices] IProblemsService problemsService, long collectionId, [FromQuery] string internalProblemId)
        {
            var problemId = await problemsService.AddAsync(collectionId, internalProblemId);
            return Ok(problemId);
        }

        public long UserId { get; set; }
        public string ApiKey { get; set; }
    }

    public class Pagination
    {
        public int Offset { get; [UsedImplicitly] set; } = 0;
        public int Limit { get; [UsedImplicitly] set; } = 10;
    }
}