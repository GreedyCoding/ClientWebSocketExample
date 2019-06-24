using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientWebSocketExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Trying to connect to websocket server");

            Uri websocketUri = new Uri("ws://demos.kaazing.com/echo");
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            ClientWebSocket webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(websocketUri, token);

            Console.WriteLine($"The websocket status is {webSocket.State.ToString()}");
            Console.WriteLine("This websocket echoes messages. If you want to exit the loop enter stop.");
            bool echoLoop = true;
            do
            {
                Console.WriteLine("Enter the message you want to echo:");

                var encoding = new UTF8Encoding();
                var buffer = new byte[1024];
                var stringToSend = Console.ReadLine();

                if (stringToSend.Equals("stop"))
                {
                    echoLoop = false;
                    stringToSend = "Attempting to close the connection. Echo! ;)";
                }

                buffer = encoding.GetBytes(stringToSend);
                ArraySegment<byte> arraySegment = new ArraySegment<byte>(buffer);

                await webSocket.SendAsync(arraySegment, WebSocketMessageType.Binary, true, token);

                WebSocketReceiveResult result = await webSocket.ReceiveAsync(arraySegment, token);

                string message = Encoding.ASCII.GetString(arraySegment.Array, arraySegment.Offset, result.Count);
                Console.WriteLine(message);
            }
            while (echoLoop);

            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", token);
            Console.WriteLine($"Connection to the websocket is now {webSocket.State.ToString()}");

            Console.Read();
        }
    }
}
