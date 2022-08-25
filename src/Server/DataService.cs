using OpenAS2UI.Shared;

namespace OpenAS2UI.Server
{
    public class DataService
    {
        private readonly HttpClient httpClient;

        public DataService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string[]> GetCertificatesAsync()
        {
            var certificates = await httpClient.GetResult<string>("cert/list").ConfigureAwait(false);

            return certificates.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray();
        }

        public async Task<string[]> GetPartnersAsync()
        {
            var partners = await httpClient.GetResult<string>("partner/list").ConfigureAwait(false);

            return partners.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray();
        }

        public async Task<string[]> GetPartnershipsAsync()
        {
            var partnerships = await httpClient.GetResult<string>("partnership/list").ConfigureAwait(false);

            return partnerships.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray();
        }

        public async Task<PartnerDefinition> GetPartnerAsync(string name)
        {
            var partner = await httpClient.GetResult<PartnerDefinition>("partner/view/" + Uri.EscapeDataString(name)).ConfigureAwait(false);

            return partner.First();
        }

        public async Task<CertificateDefinition> GetCertificateAsync(string name)
        {
            var certificate = await httpClient.GetResult<CertificateDefinition>("cert/view/" + Uri.EscapeDataString(name)).ConfigureAwait(false);

            return certificate.First();
        }

        public async Task AddPartnerAsync(PartnerDefinition partner)
        {
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, new Uri("partner/add/" + Uri.EscapeDataString(partner.Name)));
            msg.Content = new FormUrlEncodedContent(GetPartnerProperties(partner));

            await httpClient.GetResult<string>(msg); // Doesn't return anything
        }

        public Task DeletePartnerAsync(string name)
        {
            return httpClient.GetResult<string>("partner/delete/" + Uri.EscapeDataString(name));
        }

        private IEnumerable<KeyValuePair<string, string>> GetPartnerProperties(PartnerDefinition partner)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>(5)
            {
                { "name", partner.Name },
                { "as2_id" , partner.As2Id },
                { "x509_alias" , partner.X509Alias },
            };

            if (!string.IsNullOrEmpty(partner.Email))
                properties.Add("email", partner.Email);

            if (!string.IsNullOrEmpty(partner.X509AliasFallback))
                properties.Add("x509_alias_fallback", partner.X509AliasFallback);

            return properties;
        }

        public Task StorePartnershipsAsync()
        {
            return httpClient.GetResult<string>("partnership/store");
        }
    }
}
