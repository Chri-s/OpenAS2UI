using Microsoft.AspNetCore.Mvc;
using OpenAS2UI.Shared;

namespace OpenAS2UI.Server
{
    internal static class Extensions
    {
        public static async Task<T[]> GetResult<T>(this HttpClient httpClient, string uri)
        {
            using Stream stream = await httpClient.GetStreamAsync(uri).ConfigureAwait(false);

            ResultList<T> resultList = new ResultList<T>(stream);
            resultList.ThrowIfError();

            return resultList.Results;
        }

        public static async Task<T[]> GetResult<T>(this HttpClient httpClient, HttpRequestMessage request)
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);
            using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

            ResultList<T> resultList = new ResultList<T>(stream);
            resultList.ThrowIfError();

            return resultList.Results;
        }

        public static CertificateDefinition ToCertificateDefinition(this Certificate certificate)
        {
            return new CertificateDefinition()
            {
                Alias = certificate.Alias!,
                Issuer = certificate.Issuer,
                Subject = certificate.Subject,
                NotAfter = certificate.NotAfter!.Value,
                NotBefore = certificate.NotBefore!.Value,
                PublicKey = certificate.PublicKey ?? new byte[0],
                HasPrivateKey = certificate.HasPrivateKey!.Value,
                SerialNumber = certificate.SerialNumber,
            };
        }

        public static PartnerDefinition ToPartnerDefinition(this Partner partner)
        {
            return new PartnerDefinition()
            {
                As2Id = partner.As2_id!,
                Email = partner.Email,
                Name = partner.Name!,
                X509Alias = partner.X509_alias!,
                X509AliasFallback = partner.X509_alias_fallback
            };
        }

        public static CertificateImport ToCertificateImport(this CertificateImportDefinition certificateImportDefinition)
        {
            return new CertificateImport()
            {
                Alias = certificateImportDefinition.Alias,
                Password = certificateImportDefinition.Password,
                Pkcs12Container = certificateImportDefinition.Pkcs12Container,
                PublicKey = certificateImportDefinition.Certificate
            };
        }

        public static ActionResult ServiceUnavailable(this ControllerBase controller, object result)
        {
            return controller.StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }
    }
}