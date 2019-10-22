using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace MessagesTensorFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        [HttpPost]
        public HttpStatusCode Post([FromBody]StatusMessage message)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(message));
            return HttpStatusCode.NoContent;
        }
    }
}