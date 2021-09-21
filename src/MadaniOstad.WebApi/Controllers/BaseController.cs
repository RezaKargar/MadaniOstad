using MadaniOstad.IocConfig.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MadaniOstad.WebApi.Controllers
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
