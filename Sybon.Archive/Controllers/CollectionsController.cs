using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using Sybon.Archive.Services.CollectionsService;
using Sybon.Archive.Services.CollectionsService.Models;
using Sybon.Archive.Services.ProblemsService;
using Sybon.Common;

namespace Sybon.Archive.Controllers
{
    [Route("api/[controller]")]
    public class CollectionsController : Controller, ILogged
    {
        [HttpGet]
        [SwaggerOperation("Get")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ICollection<ProblemCollectionWithoutProblems>))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        public async Task<IActionResult> Get([FromServices] ICollectionsService collectionsService, [FromQuery] Pagination pagination)
        {
            var collections = await collectionsService.GetRangeAsync(pagination.Offset, pagination.Limit);
            return Ok(collections);
        }

        [HttpGet("{id}")]
        [SwaggerOperation("GetById")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(ProblemCollectionWithProblems))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        [CollectionPermissionFilter(CollectionPermissionFilterAttribute.Type.Read, "id")]
        public async Task<IActionResult> Get([FromServices] ICollectionsService collectionsService, long id)
        {
            return Ok(await collectionsService.FindAsync(id));
        }

        [HttpPost]
        [SwaggerOperation("Add")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter(true)]
        public async Task<IActionResult> Add([FromServices] ICollectionsService collectionsService, [FromBody] CollectionForm collection)
        {
            var id = await collectionsService.AddAsync(UserId, collection);
            return Ok(id);
        }

        [HttpDelete]
        [SwaggerOperation("Delete")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter(true)]
        public async Task<IActionResult> DeleteCollections([FromServices] ICollectionsService collectionsService, [FromBody] long[] collectionIds)
        {
            await collectionsService.RemoveRangeAsync(collectionIds);
            return Ok();
        }

        [HttpPost("{collectionId}/problems")]
        [SwaggerOperation("AddProblem")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        [CollectionPermissionFilter(CollectionPermissionFilterAttribute.Type.Write, "collectionId")]
        public async Task<IActionResult> AddProblem([FromServices] IProblemsService problemsService, long collectionId, [FromQuery] string internalProblemId)
        {
            var problemId = await problemsService.AddAsync(collectionId, internalProblemId);
            return Ok(problemId);
        }
        
        [HttpDelete("{collectionId}/problems")]
        [SwaggerOperation("DeleteProblems")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(long))]
        [SwaggerOperationFilter(typeof(SwaggerApiKeySecurityFilter))]
        [AuthorizeFilter]
        [CollectionPermissionFilter(CollectionPermissionFilterAttribute.Type.Write, "collectionId")]
        public async Task<IActionResult> DeleteProblems([FromServices] IProblemsService problemsService, long collectionId, [FromBody] long[] problemIds)
        {
            await problemsService.RemoveRangeAsync(problemIds);
            return Ok();
        }

        public long UserId { get; set; }
        public string ApiKey { get; set; }

        public class Pagination
        {
            public int Offset { get; [UsedImplicitly] set; } = 0;
            public int Limit { get; [UsedImplicitly] set; } = 10;
        }
    }
}