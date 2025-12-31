using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerakiApiPlayground
{
    using Newtonsoft.Json;
    using System.IO;

    public class Archiver
    {
        public static void SaveDevice(Device device)
        {
            var folder = $"C:/archive/{DateTime.Now:yyyy-MM-dd}/devices";
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, $"{device.Serial}.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(device, Formatting.Indented));
        }

        public static void SaveSwitchPorts(string serial, string json)
        {
            var folder = $"archive/{DateTime.Now:yyyy-MM-dd}/configs";
            Directory.CreateDirectory(folder);
            var path = Path.Combine(folder, $"{serial}_ports.json");
            File.WriteAllText(path, json);
        }
    }
}
