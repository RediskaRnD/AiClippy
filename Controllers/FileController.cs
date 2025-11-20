using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Persistence.Repositories.Interfaces;
using Persistence.Extensions;

namespace Controllers
{
    // [Authorize]
    [Route("[controller]")]
    public class FileController(IFileRepository repository, IConfiguration configuration) : BaseApiController
    {
        [HttpGet(nameof(GetFileInfoById))]
        public async Task<string> GetFileInfoById([FromQuery] long id, CancellationToken ct)
        {
            var file = await repository.GetFileInfoByIdAsync(id, ct);
            var json = file?.ToJson();
            
            return json ?? "No file data found";
        }
    }
}