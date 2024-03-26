using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;


namespace BlockchainPeerNode;

class Program
{
    public static List<string> Peers = new List<string>();
     
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Listen();
        Console.ReadKey();
        Console.WriteLine("yeaa");
        Console.ReadKey();
    }

    public static async Task Listen()
    {
        IPEndPoint ipEndPoint = new IPEndPoint(new IPAddress(IPAddress.Parse("192.168.0.143").GetAddressBytes()), 1202);
        
        using Socket listener = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        listener.Bind(ipEndPoint);
        listener.Listen(100);
        while (true)
        {


            var handler = await listener.AcceptAsync();
            while (true)
            {
                // Receive message.
                var buffer = new byte[1_024];
                var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                var response = Encoding.UTF8.GetString(buffer, 0, received);

                Console.WriteLine("Socket received: " + response);

                if (Peers.Contains(response)) Peers.Remove(response);
                
                var ackMessage = JsonSerializer.Serialize(Peers);
                var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                await handler.SendAsync(echoBytes, 0);
                Console.WriteLine(
                    $"Socket server sent acknowledgment: \"{ackMessage}\"");

                Peers.Add(response);

                break;

            }
        }
    }
}