
//  Was originally MerakiPowerCycler. Meraki Dashboard API doesn't support power cycling of Catalyst switches.
//  Reporposed to list wireless clients instead, along with general API client testing and learning.

using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography;

namespace MerakiApiPlayground
{

    class Program
    {
        private const string ApiKey = "494dd105d5612ee2b3478b302465431e352c74f6";
        private const string BaseUrl = "https://api.meraki.com/api/v1";
        private const string NetworkId = "L_683421243453475514";
        private const string OrgId = "519471";
        private const string TargetNetworkName = "14-FMIG";

        static void Main()
        {
            var devices = MerakiApi.GetDevices(OrgId);
            Console.WriteLine($"Found {devices.Count} devices.");

            ListDevices(devices);


            Console.WriteLine("Archiving complete.");
            Console.ReadKey();
        }

        private static void ListDevices(List<Device> devices)
        {
            foreach (var device in devices)
            {
                try
                {
                    Archiver.SaveDevice(device);
                    Console.Write($"{device.Name}: \t");
                    var portJson = MerakiApi.GetSwitchPorts(device.Serial);
                    if (!string.IsNullOrEmpty(portJson))
                    {
                        Archiver.SaveSwitchPorts(device.Serial, portJson);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write($"Collected.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("Empty.");
                    }
                }
                catch
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Error.");
                }
                finally
                { Console.ResetColor(); Console.WriteLine(); }
            }
        }
    }

    

    class MerakiClient
    {
        private const string NetworkId = "L_683421243453475514";
        private const string OrgId = "519471";
        private const string TargetNetworkName = "14-FMIG";
        public static void MainFunc()
        {
            //MerakiNetwork.FinderListNetworkID();
            var clients = GetWirelessClients(NetworkId);
            foreach (var client in clients)
            {
                Console.WriteLine($"MAC: {client.Mac}, IP: {client.Ip}, Description: {client.Description}, Status: {client.Status}, Connection Type: {client.RecentDeviceConnection}, deviceTypePrediction: {client.deviceTypePrediction}, SSID: {client.ssid}, User: {client.user}");
            }
            var pause = Console.ReadKey();
        }

        public static List<Client> GetWirelessClients(string networkId)
        {
        string ApiKey = "494dd105d5612ee2b3478b302465431e352c74f6";
        string BaseUrl = "https://api.meraki.com/api/v1";
        var client = new RestClient(BaseUrl);
            var request = new RestRequest($"/networks/{networkId}/clients", Method.Get);
            request.AddHeader("X-Cisco-Meraki-API-Key", ApiKey);
            request.AddParameter("timespan", 300); // Last 5 minutes (300) 24 hours (86400)
            request.AddParameter("perPage", 1000); // Max clients per page

            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
                return new List<Client>();
            }

            return JsonConvert.DeserializeObject<List<Client>>(response.Content);
        }

    }

    class MerakiNetworkFinder
    {
        private const string ApiKey = "494dd105d5612ee2b3478b302465431e352c74f6";
        private const string OrgId = "519471";
        private const string TargetNetworkName = "14-FMIG";

        public static void ListNetworkID()
        {
            var networkId = GetNetworkIdByName(OrgId, TargetNetworkName);
            Console.WriteLine($"Network ID for '{TargetNetworkName}': {networkId}");
        }

        public static string GetNetworkIdByName(string orgId, string networkName)
        {
            var client = new RestClient("https://api.meraki.com/api/v1");
            var request = new RestRequest($"/organizations/{orgId}/networks", Method.Get);
            request.AddHeader("X-Cisco-Meraki-API-Key", ApiKey);

            var response = client.Execute(request);
            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
                return null;
            }

            var networks = JsonConvert.DeserializeObject<List<Network>>(response.Content);
            foreach (var network in networks) { Console.WriteLine(network.Name + ":\t" + network.Id); }
            var match = networks.FirstOrDefault(n => n.Name == networkName);
            return match?.Id;
        }


    }
}
//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;

//class MerakiApiExample
//{
//    static async Task Main()
//    {
//        string apiKey = "494dd105d5612ee2b3478b302465431e352c74f6\r\n";
//        string serial = "FOC2503LCKQ".Trim(); //IDF18 switch 2 ; gi2/0/1
//        apiKey = apiKey.Trim(); // Removes leading/trailing whitespace and newlines
//        string url = $"https://api.meraki.com/api/v1/devices/{serial}/switch/ports/cycle";

//        var client = new HttpClient();
//        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
//        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//        var json = "{\"ports\": [\"3\"]}"; // Power cycle port 1
//        var content = new StringContent(json, Encoding.UTF8, "application/json");

//        var response = await client.PostAsync(url, content);
//        string result = await response.Content.ReadAsStringAsync();

//        Console.WriteLine(content.ToString());
//        Console.WriteLine($"Response: {response.StatusCode}");
//        Console.WriteLine(result);

//        await ListDevices(client);

//        var pause = Console.ReadKey();
//    }

//    static async Task ListDevices(HttpClient? client)
//    {
//        string networkId = "14-FMIG";
//        string url = $"https://api.meraki.com/api/v1/networks/{networkId}/devices";

//        var response = await client!.GetAsync(url);
//        string result = await response.Content.ReadAsStringAsync();
//        Console.WriteLine(result);
//    }
//}