using Business.Services;
using Common.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAPIs.Filters;

namespace MyAPIs.Controllers
{
    [TypeFilter(typeof(LogRequest))]
    [ApiController]
    [Route("[controller]")]
    public class TransferController : Controller
    {
        private readonly ILogger<TransferController> _logger;
        //private readonly IConfiguration _configuration;
        private readonly ITransferService _transferService;
        public TransferController(ILogger<TransferController> logger, /*IConfiguration configuration,*/ ITransferService transferService)
        {
            _logger = logger;
            //_configuration = configuration;
            _transferService = transferService;
        }

        // GET: TransferController
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            _logger.LogInformation("Index action called in TransferController");
            _transferService.Transfer(new BaseRequest(){CorrelationId = Guid.NewGuid().ToString()});
            return Ok();
        }

        // GET: TransferController/Details/5
        [HttpGet]
        [Route("Details/{id}")]
        public ActionResult Details(int id)
        {
            return Ok();
        }

        // GET: TransferController/Create
        [HttpPost]
        [Route("Create")]
        public ActionResult Create()
        {
            return Ok();
        }

        // POST: TransferController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateWithToken")]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {

                return Ok(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }

        // GET: TransferController/Edit/5
        [HttpPut]
        [Route("{id}")]
        public ActionResult Edit(int id)
        {
            return Ok();
        }

        // POST: TransferController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }

        // GET: TransferController/Delete/5
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            return Ok();
        }

        // POST: TransferController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return Ok();
            }
        }
    }
}
