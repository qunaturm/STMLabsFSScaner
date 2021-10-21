using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace STMLabsFSScaner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FSScanerController : ControllerBase
    {
        private readonly ILogger<FSScanerController> _logger;
        private readonly Domain.FSScaner _fsScanner;
        public FSScanerController(ILogger<FSScanerController> logger, Domain.FSScaner fsScanner)
        {
            _logger = logger;
            _fsScanner = fsScanner;
        }
        [HttpGet]
        public ActionResult<Domain.Folder> Get([FromQuery] string path)
        {
            var folder = _fsScanner.GetFolder(path);
            if (folder != null)
            {
                return Ok(folder);
            }
            else return NotFound();
        }

        [HttpPost]
        public ActionResult Post([FromQuery] string path)
        {
            _fsScanner.ScanFolder(path);
            var result = _fsScanner.GetFolder(path);
            return Ok(result);
        }
    }
}
