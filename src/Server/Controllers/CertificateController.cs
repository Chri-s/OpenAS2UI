using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using OpenAS2UI.Shared;

namespace OpenAS2UI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CertificateController : ControllerBase
    {
        private DataService _dataService;

        public CertificateController(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            string[]? certificates = null;
            try
            {
                certificates = await _dataService.GetCertificatesAsync();
            }
            catch (HttpRequestException ex)
            {
                return this.BadRequest(ex.Message);
            }

            return this.Ok(certificates);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(string id)
        {
            CertificateDefinition? certificate = null;
            try
            {
                certificate = await _dataService.GetCertificateAsync(id);

                if (certificate != null)
                {
                    try
                    {
                        using (X509Certificate2 cert = new X509Certificate2(certificate.Data))
                        {
                            certificate.SerialNumber = cert.SerialNumber;
                            certificate.Issuer = cert.Issuer;
                            certificate.NotAfter = cert.NotAfter;
                            certificate.NotBefore = cert.NotBefore;
                            certificate.Thumbprint = cert.Thumbprint;
                        }
                        
                    }
                    catch
                    {
                        // Do nothing, couldn't parse certificate
                    }
                }
            }
            catch (ResultLoadingException ex)
            {
                if (ex.Message.Trim().Equals("Unknown partner name", StringComparison.OrdinalIgnoreCase))
                    return this.NotFound(ex.Message);

                return this.BadRequest(ex.Message);
            }

            return this.Ok(certificate);
        }

        [HttpGet("download/{id}")]
        public async Task<ActionResult> Download(string id)
        {
            CertificateDefinition? certificate = null;
            try
            {
                certificate = await _dataService.GetCertificateAsync(id);

                if (certificate != null)
                {
                    StringBuilder data = new StringBuilder("-----BEGIN CERTIFICATE-----");
                    data.AppendLine();

                    string base64 = Convert.ToBase64String(certificate.Data);

                    for (int i = 0; i < base64.Length; i += 64)
                    {
                        if (base64.Length > i + 64)
                            data.AppendLine(base64.Substring(i, 64));
                        else
                            data.AppendLine(base64.Substring(i));
                    }

                    data.AppendLine("-----END CERTIFICATE-----");

                    return this.File(Encoding.UTF8.GetBytes(data.ToString()), "application/x-x509-ca-cert", id + ".cer");
                }
            }
            catch (ResultLoadingException ex)
            {
                if (ex.Message.Trim().Equals("Unknown partner name", StringComparison.OrdinalIgnoreCase))
                    return this.NotFound(ex.Message);

                return this.BadRequest(ex.Message);
            }

            return this.NotFound();
        }
    }
}
