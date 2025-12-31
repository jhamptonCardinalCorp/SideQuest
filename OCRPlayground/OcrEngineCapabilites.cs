using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRPlayground
{
    // Yes, I've put "Orc" by mistake more than once.
    public class OcrEngineCapabilites
    {
        public bool SupportsGpu { get; set; }
        public bool SupportsLanguages { get; set; }
        public IEnumerable<string> AvailableLanguages { get; set; } = Enumerable.Empty<string>();
    }
}
