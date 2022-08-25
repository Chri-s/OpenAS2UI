using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OpenAS2UI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnershipController : ControllerBase
    {
        private DataService _dataService;

        public PartnershipController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string[]? partnerships = null;
            try
            {
                partnerships = await _dataService.GetPartnershipsAsync();
            }
            catch (HttpRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }

            return this.Ok(partnerships);
        }
    }
}
