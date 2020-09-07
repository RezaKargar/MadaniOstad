using KodoomOstad.IocConfig.Filters;
using Microsoft.AspNetCore.Mvc;

namespace KodoomOstad.WebApi.Controllers
{
    [ApiController]
    [ApiResultFilter]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Consumes("application/json", "application/x-www-form-urlencoded", "multipart/form-data")]
    [Produces("application/json")]
    public class BaseController : ControllerBase
    {
    }
}
