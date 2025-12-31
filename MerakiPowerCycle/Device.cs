using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerakiApiPlayground
{
    public class Device
    {
        public string Name { get; set; }
        public string Serial { get; set; }
        public string Mac { get; set; }
        public string Model { get; set; }
        public string NetworkId { get; set; }
        public string Firmware { get; set; }
        public string Address { get; set; }
        public string LanIp { get; set; }
        public string PublicIp { get; set; }
    }
}
