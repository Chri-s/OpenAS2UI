using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAS2UI.Shared
{
    public class CertificateImportDefinition
    {
        public string Alias { get; set; } = string.Empty;

        public byte[]? Certificate { get; set; }

        public byte[]? Pkcs12Container { get; set; }

        public string? Password { get; set; }
    }
}
