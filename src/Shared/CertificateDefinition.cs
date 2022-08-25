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

        [JsonPropertyName("data")]
        public byte[] Data { get; set; } = new byte[0];

        public string? Issuer { get; set; }

        public DateTime NotBefore { get; set; }

        public DateTime NotAfter { get; set; }

        public string? SerialNumber { get; set; }

        public string? Thumbprint { get; set; }
    }
}
