using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenAS2UI.Shared
{
    public class CertificateDefinition
    {
        [JsonPropertyName("alias")]
        public string Alias { get; set; } = string.Empty;

        [JsonPropertyName("publicKey")]
        public byte[] PublicKey { get; set; } = new byte[0];

        [JsonPropertyName("issuer")]
        public string? Issuer { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        public DateTimeOffset NotBefore { get; set; }

        public DateTimeOffset NotAfter { get; set; }

        public bool HasPrivateKey { get; set; }

        public string? SerialNumber { get; set; }

        public List<PartnerDefinition>? UsedBy { get; set; }
    }
}
