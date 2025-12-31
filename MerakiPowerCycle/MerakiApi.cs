using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerakiApiPlayground
{
    using RestSharp;
    using Newtonsoft.Json;

    public class MerakiApi
    {
        private const string ApiKey = "494dd105d5612ee2b3478b302465431e352c74f6";
        private const string BaseUrl = "https://api.meraki.com/api/v1";

        public static List<Device> GetDevices(string orgId)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"/organizations/{orgId}/devices", Method.Get);
            request.AddHeader("X-Cisco-Meraki-API-Key", ApiKey);

            var response = client.Execute(request);
            return response.IsSuccessful
                ? JsonConvert.DeserializeObject<List<Device>>(response.Content)
                : new List<Device>();
        }

        public static string GetSwitchPorts(string serial)
        {
            var client = new RestClient(BaseUrl);
            var request = new RestRequest($"/devices/{serial}/switch/ports", Method.Get);
            request.AddHeader("X-Cisco-Meraki-API-Key", ApiKey);

            var response = client.Execute(request);
            return response.IsSuccessful ? response.Content : null;
        }
    }
}
