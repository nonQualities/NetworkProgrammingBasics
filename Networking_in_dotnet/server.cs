using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    public static async Task Main()
    {
        IPEndPoint iPEndPoint = new(IPAddress.Any, 8080);

        using Socket server = new(
            iPEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        server.Bind(iPEndPoint);
        server.Listen(100);
        Console.WriteLine("Server is Listening...");



        using Socket handler = await server.AcceptAsync();
        Console.WriteLine("Server Connected to {iPEndPoint}");

        while (true)
        {
            var buffer = new byte[1_024];
            var recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, recieved);
            const byte eom = "<|EOM|>";

            if (response.IndexOf(eom) > -1)
            {
                Console.WriteLine(
                    $"Socket server recieved message: {response.Replace(eom, " ")}"
                );

                var ackMessage = "<|ACK|>";
                var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
                await handler.SendAsync(echoBytes, 0);
                Console.WriteLine(
                    $"Socket server sent acknowledgment: \"{ackMessage}\"");
        
                break;
            }

        }

    }
}