//using Newtonsoft.Json;
//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MerakiApiPlayground
//{
//    public class IPCrawler
//    {
//        public static List<Device> GetDevices(string orgId)
//        {
//            var client = new RestClient(BaseUrl);
//            var request = new RestRequest($"/organizations/{orgId}/devices", Method.Get);
//            request.AddHeader("X-Cisco-Meraki-API-Key", ApiKey);

//            var response = client.Execute(request);
//            if (!response.IsSuccessful)
//            {
//                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
//                return new List<Device>();
//            }

//            return JsonConvert.DeserializeObject<List<Device>>(response.Content);
//        }

//        public static string GetSwitchPorts(string serial)
//        {
//            var client = new RestClient(BaseUrl);
//            var request = new RestRequest($"/devices/{serial}/switch/ports", Method.Get);
//            request.AddHeader("X-Cisco-Meraki-API-Key", ApiKey);

//            var response = client.Execute(request);
//            return response.IsSuccessful ? response.Content : null;
//        }

//        File.WriteAllText($"configs/{serial}_ports.json", portJson);
//    }
//}
