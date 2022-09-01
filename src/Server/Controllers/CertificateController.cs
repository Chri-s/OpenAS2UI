using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAS2UI.Shared;

namespace OpenAS2UI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificateController : ControllerBase
    {
        private OpenAs2Client _client;

        public CertificateController(OpenAs2Client client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            ICollection<Certificate>? certificates = null;
            try
            {
                certificates = await _client.CertificatesAllAsync();
            }
            catch (ApiException<ErrorObject> ex)
            {
                return this.StatusCode(ex.StatusCode, ex.Result);
            }
            catch (HttpRequestException ex)
            {
                return this.ServiceUnavailable(new ErrorObject() { ErrorMessage = ex.Message });
            }

            return this.Ok(certificates.Select(c => c.ToCertificateDefinition()));
        }

        [HttpGet("{alias}/")]
        public async Task<ActionResult> Get(string alias)
        {
            CertificateDefinition certificateDefinition;
            try
            {
                Certificate certificate = await _client.CertificatesGETAsync(alias);

                certificateDefinition = certificate.ToCertificateDefinition();
                certificateDefinition.UsedBy = (await _client.UsedbyAsync(alias)).Select(p => p.ToPartnerDefinition()).ToList();

                return this.Ok(certificateDefinition);
            }
            catch (ApiException<ErrorObject> ex)
            {
                return this.StatusCode(ex.StatusCode, ex.Result);
            }
            catch (HttpRequestException ex)
            {
                return this.ServiceUnavailable(new ErrorObject() { ErrorMessage = ex.Message });
            }
        }

        [HttpPost("{alias}/privatekey/")]
        public async Task<ActionResult> DownloadPrivateKey(string alias, [FromForm] string exportPassword)
        {
            try
            {
                Certificate certificate = await _client.PrivatekeyAsync(alias, new Pkcs12Export() { ExportPassword = exportPassword });

                return File(certificate.Pkcs12Container!, "application/octet-stream");
            }
            catch (ApiException<ErrorObject> ex)
            {
                return this.StatusCode(ex.StatusCode, ex.Result);
            }
            catch (HttpRequestException ex)
            {
                return this.ServiceUnavailable(new ErrorObject() { ErrorMessage = ex.Message });
            }
        }

        [HttpDelete("{alias}/")]
        public async Task<ActionResult> DeleteCertificate(string alias)
        {
            try
            {
                await _client.CertificatesDELETEAsync(alias);

                return Ok();
            }
            catch (ApiException<ErrorObject> ex)
            {
                return this.StatusCode(ex.StatusCode, ex.Result);
            }
            catch (HttpRequestException ex)
            {
                return this.ServiceUnavailable(new ErrorObject() { ErrorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> ImportCertificate(CertificateImportDefinition certificateImport)
        {
            try
            {
                Certificate certificate = await _client.CertificatesPOSTAsync(certificateImport.ToCertificateImport());

                return Ok(certificate);
            }
            catch (ApiException<ErrorObject> ex)
            {
                return this.StatusCode(ex.StatusCode, ex.Result);
            }
            catch (HttpRequestException ex)
            {
                return this.ServiceUnavailable(new ErrorObject() { ErrorMessage = ex.Message });
            }
        }
    }
}
