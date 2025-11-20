using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    // [Authorize]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
    }
}