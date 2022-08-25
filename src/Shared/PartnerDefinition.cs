using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAS2UI.Shared
{
    public class PartnerDefinition
    {
        [JsonPropertyName("name")]
        [Required(ErrorMessage = "You must specify a name.")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("as2_id")]
        [Required(ErrorMessage = "You must specify a AS2 id.")]
        public string As2Id { get; set; } = string.Empty;

        [JsonPropertyName("x509_alias")]
        [Required(ErrorMessage = "You must select a certificate.")]
        public string X509Alias { get; set; } = string.Empty;

        [JsonPropertyName("x509_alias_fallback")]
        public string? X509AliasFallback { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("known_certificates")]
        public List<string>? KnownCertificates { get; set; }
    }
}
