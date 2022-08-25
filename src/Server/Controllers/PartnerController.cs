using Microsoft.AspNetCore.Mvc;
using OpenAS2UI.Shared;

namespace OpenAS2UI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PartnerController : ControllerBase
    {
        private DataService _dataService;

        public PartnerController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string[]? partners = null;
            try
            {
                partners = await _dataService.GetPartnersAsync();
            }
            catch (HttpRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }

            List<string> partList = new List<string>(1000 * partners.Length);

            for (int i = 0; i < 1000; i++)
            {
                partList.AddRange(partners.Select(s => s + " " + ((i > 0) ? i.ToString() : "")));
            }

            return this.Ok(partList.ToArray());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            PartnerDefinition? partner = null;
            try
            {
                partner = await _dataService.GetPartnerAsync(id);

                if (partner != null)
                    partner.KnownCertificates = (await _dataService.GetCertificatesAsync()).ToList();
            }
            catch (ResultLoadingException ex)
            {
                if (ex.Message.Trim().Equals("Unknown partner name", StringComparison.OrdinalIgnoreCase))
                    return this.NotFound(ex.Message);

                return this.BadRequest(ex.Message);
            }

            return this.Ok(partner);
        }
    }
}
