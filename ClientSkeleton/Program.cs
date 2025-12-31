////         ClientSkeleton          //
////---------------------------------//
////
//I'm trying to pull a data stream from Kantech entrapass's smartlink. I can confirm that the username and password work, and have even tried another set that I also know work. I can use powershell on the server and see the client connecting to the socket, and I've confirmed that it's the port smartlink is using. Yet, it keeps timing out with no response.

// I'm not sure if I'm missing a step in the authentication process, or if there's something else going on. Any help would be appreciated!
//  We could try packet sniffing the login process with Wireshark to see what the client is sending and compare it to what our code is sending.
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SmartLinkWrapper;

namespace ClientSkeleton
{
    class Program
    {
        static void Main()
        {
            var ipAddress = "10.14.7.10";
            var tag = 1;
            var username = "SmartLink";
            var password = "Fremont14@@";

            // Step 1: Connect and prepare authentication
            SmartDll.AutomaticConnect(tag, ipAddress);

            // Step 2: Build login command
            byte[] loginCommand = new byte[1024];
            int resultLength;
            int buildResult = SmartDll.BuildLoginPassword(username, password, loginCommand.Length, out resultLength, loginCommand);

            if (buildResult != 0)
            {
                Console.WriteLine("Failed to build login command.");
                return;
            }

            // Step 3: Send login command
            int loginResult = SmartDll.SetLoginPassword(resultLength, loginCommand);
            if (loginResult != 0)
            {
                Console.WriteLine($"Login failed. Error code: {loginResult}");
                return;
            }

            // Step 4: Verify connection
            if (SmartLinkHelper.VerifyConnection())
            {
                Console.WriteLine("Connected and logged in.");
                string response = SmartDll.SendCommand("Command=GetCardInfoCardNumber=12345");
                Console.WriteLine($"Response: {response}");
            }
            else
            {
                Console.WriteLine("Failed to connect or login.");
                Console.WriteLine($"SmartLinkState: {SmartDll.GetSmartLinkState()}");
                Console.WriteLine($"ServerState: {SmartDll.GetServerState()}");
                Console.WriteLine($"LoginState: {SmartDll.GetLoginState()}");
            }

            SmartDll.Disconnect();
            Console.WriteLine("Disconnected.");
            _ = Console.ReadKey();
        }
        
        //static void Main()
        //{
        //    var ipAddress = "10.14.7.10";
        //    var tag = 1;
        //    var username = "SmartLink";
        //    var password = "Fremont14@@";

        //    //SmartDll.Connect(tag, ipAddress);
        //    //SmartDll.Login(username, password);
        //    // Step 1: Build login command
        //    //byte[] loginCommand = new byte[1024];
        //    //int resultLength;
        //    SmartDll.AutomaticConnect(tag, ipAddress);  //new
        //    SmartDll.BuildLoginPassword(username, password, loginCommand.Length, out resultLength, loginCommand);

        //    // Step 2: Set login command
        //    SmartDll.SetLoginPassword(resultLength, loginCommand);

        //    // Step 3: Connect and authenticate
        //    SmartDll.AutomaticConnect(tag, ipAddress);
        //    if (SmartLinkHelper.VerifyConnection())
        //    {
        //        Console.WriteLine("Connected to SmartLink server.");
        //        //var loginSuccess = SmartDll.Login(username, password);
        //        //Console.WriteLine($"Login Success: {loginSuccess}");
        //    }
        //    else {
        //        Console.WriteLine("Failed to connect to Smartlink server.");
        //        Console.WriteLine($"SmartLinkState: {SmartDll.GetSmartLinkState()}");
        //        Console.WriteLine($"ServerState: {SmartDll.GetServerState()}");
        //        Console.WriteLine($"LoginState: {SmartDll.GetLoginState()}");
        //            }
        //    var par = Console.ReadLine();

        //    //SmartDll.Disconnect();
        //    //while (true)
        //    //{
        //    //    var message = PollMessage();
        //    //    if (!string.IsNullOrWhiteSpace(message)) { Console.WriteLine(message); }
        //    //    Task.Delay(1000).Wait();
        //    //}

        //    }
        public static string PollMessage()
        {
            byte[] buffer = new byte[1024];
            int length;
            int result = SmartDll.GetMessageData(buffer.Length, out length, buffer);

            if (result == 0 && length > 0)
            {
                return Encoding.ASCII.GetString(buffer, 0, length);
            }

            return string.Empty;
        }
    }

class SmartLinkListener
    {
        public void Start()
        {
            using var client = new TcpClient("kantech-server-ip", 18104);
            using var stream = client.GetStream();

            // Send login packet (you'll need to format this correctly)
            byte[] loginPacket = GetLoginPacket("username", "password");
            stream.Write(loginPacket, 0, loginPacket.Length);

            // Read incoming messages
            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    // Parse the message
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: " + message);
                }
            }
        }

        private byte[] GetLoginPacket(string user, string pass)
        {
            // You’ll need to reverse-engineer or capture a valid login packet
            // from SmartLink to know the correct format
            return new byte[] { /* formatted login bytes */ };
        }
    }
    //public class SmartLinkClient
    //{
    //    public event Action<SmartEvent> OnEventReceived;

    //    public void Connect(string host, int port) { /* ... */ }
    //    public void Login(string user, string pass) { /* ... */ }
    //    public void StartListening() { /* ... */ }
    //}

    public static class SmartLinkHelper
    {
        public static bool VerifyConnection()
        {
            int smartLinkState = SmartDll.GetSmartLinkState();   // 1 = connected
            int serverState = SmartDll.GetServerState();         // 1 = reachable
            int loginState = SmartDll.GetLoginState();           // 1 = logged in

            return smartLinkState == 1 && serverState == 1 && loginState == 1;
        }
    }

}
    




            //SmartDll.GetCardInfo("1234:5678");
            //SmartDll.ModifyCard("1234:5678", "John Doe");

            //string[] logs = SmartDll.PollLogMessages();

            //  This is more advanced and requires WinForms or WPF window:
            //[DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
            //public static extern int SetPostMessage(IntPtr hwnd, int msg);