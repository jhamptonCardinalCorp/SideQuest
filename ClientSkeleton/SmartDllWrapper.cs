
// 
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartLinkWrapper
{
    public static class SmartDll
    {
        // P/Invoke declarations
        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int Connect(int smartlinkTag, string ipAddress);

        //  smartlinkTag must be an integer between 0 and 65000.
        //  It identifies the client connection to SmartLink.
        //  If you're sending messages using $r (network destination variable), this tag determines which client
        //  recieves the message: $r0$r-> broadcast to *all* clients. $r25$r -> send to client with tag 25.
        //  Important: Only one connection per tag is allowed. If you reuse a tag, the previous connection
        //  will be invalidated.

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int Disconnect();

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int AutomaticConnect(int smartlinkTag, string ipAddress);

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetSmartLinkState();   //  returns 1 if connected, 0 if not

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetServerState();  //  returns 1 if server is reachable

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetLoginState();   //  returns 1 if connected, 0 if not

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SendData(
            int lengthSend,
            byte[] bufferSend,
            ref int lengthReceive,
            byte[] bufferReceive
        );

        // This function is used to send a command to SmartLink that includes a picture, typically for cardholder data.
        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SendDataPicture(
            int lengthSend,
            byte[] bufferSend,
            ref int lengthReceive,
            byte[] bufferReceive,
            string picturePath
        );

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetMessageData(
            int bufferSize,
            out int messageLength,
            byte[] buffer
        );

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int BuildLoginPassword(
            string username,
            string password,
            int resultSize,
            out int resultLength,
            byte[] result
            //StringBuilder result
        );

        [DllImport("SmartDLL.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int SetLoginPassword(
            int commandLength,
            byte[] command
        );

        // High-level helper methods
        //public static bool Login(string username, string password)
        //{
        //    StringBuilder loginCommand = new StringBuilder(1024);
        //    int resultLength;
        //    int buildResult = BuildLoginPassword(username, password, loginCommand.Capacity, out resultLength, loginCommand);
        //    if (buildResult != 0) return false;

        //    byte[] loginBytes = Encoding.ASCII.GetBytes(loginCommand.ToString());
        //    int setResult = SetLoginPassword(loginBytes.Length, loginBytes);
        //    return setResult == 0;
        //}

        public static string SendCommand(string command)
        {
            byte[] sendBuffer = Encoding.ASCII.GetBytes(command);
            byte[] receiveBuffer = new byte[4096];
            int receiveLength = receiveBuffer.Length;
            int result = SendData(sendBuffer.Length, sendBuffer, ref receiveLength, receiveBuffer);
            return result == 0 ? Encoding.ASCII.GetString(receiveBuffer, 0, receiveLength) : null;
        }

        public static string GetCardInfo(string cardNumber)
        {
            string command = $"Command=GetCardInfoCardNumber={cardNumber}";
            return SendCommand(command);
        }

        public static bool ModifyCard(string cardNumber, string userName)
        {
            string command = $"Command=ModifyCardCardNumber={cardNumber}UserName={userName}";
            string response = SendCommand(command);
            return response != null && response.Contains("SMARTLINK_COMMAND_OK");
        }

        public static string[] PollLogMessages()
        {
            byte[] buffer = new byte[4096];
            int messageLength;
            int result = GetMessageData(buffer.Length, out messageLength, buffer);
            if (result != 0 || messageLength == 0) return Array.Empty<string>();
            string message = Encoding.ASCII.GetString(buffer, 0, messageLength);
            return message.Split(new[] { '' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
