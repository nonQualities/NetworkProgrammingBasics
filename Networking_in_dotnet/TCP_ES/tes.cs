using System.Net;
using System.Net.Sockets;
using System.Text;

public class Tcp_Server{

    public static async Task Main()
    {
        const int PORT = 8080;
        var ACK = Encoding.UTF8.GetBytes("<|ACK|>");
        const string EOM = "<|EOM|>";

        
        IPEndPoint iPEndPoint = new(IPAddress.Any, PORT);

        /* the using statement disposes resouces no longer in use 
        * it implements the IDisposable interface
        * it disposes resources at the end of enclosing scope
        */
        using Socket server = new(
            iPEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        server.Bind(iPEndPoint);
        server.Listen(100);
        Console.WriteLine($"[INFO] Server is Listening");

        using Socket handler = await server.AcceptAsync();
        Console.WriteLine($"[INFO] The Server is listening to {iPEndPoint}");
        Console.WriteLine("-----------------------------------------");

        while (true)
        {
            var bfr = new byte[2048];
            var recieve = await handler.ReceiveAsync(bfr, SocketFlags.None); //a Task<TResult> if succesfull contains the count of bytes recieved
            var response = Encoding.UTF8.GetString(bfr, 0, recieve); // 0 here is the index, and recieve is the count

            if (response.IndexOf(EOM) > -1)
            {
                Console.WriteLine($"[MSG]: {response}");
                await handler.SendAsync(ACK);
            }
            if (response == ("END"+EOM))
            {
                var end = "END" + EOM;
                var END = Encoding.UTF8.GetBytes(end); 
                await handler.SendAsync(END);
                break;
            }

        }

        handler.Shutdown(SocketShutdown.Both); 
        handler.Close();
        server.Shutdown(SocketShutdown.Both); 
        server.Close();

    }
}
