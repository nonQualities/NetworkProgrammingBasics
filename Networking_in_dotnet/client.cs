using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client
{
    public static async Task Main()
    {
        /*
        * In dotnet the process for creating a socket goes through some OO bullshit but 
        * is not that bad nevertheless:
        * We need to create a IPEndpint(ipAdress, port)
        * Then we need to setup an socket obj 
        * then we can do all other stuff like sending and recieving
        */



        IPAddress localIpAddress = IPAddress.Parse("127.0.0.1");


        Console.WriteLine($"IPAdress is : {localIpAddress}");
        IPEndPoint iPEndPoint = new(localIpAddress, 8080);

        //there are some extra baggage when sending message but it makes it safer
        // Like encoding it to UTF-8

        /*
        * this is the socket constructor: it has many overloads ofc, but this is the main one.
        * another one is Socket(SocketHandle);
        * A socket handle is the refernce to the socket object
        */
        using Socket Client = new(
            iPEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );

        await Client.ConnectAsync(iPEndPoint);
        Console.WriteLine("------");


        const string eom = "<|EOM|>";
        var m = Console.ReadLine();
        var message = m + eom;
        var bytes = Encoding.UTF8.GetBytes(message);
        var buffer = new byte[1_024];

        while (true)
        {
            await Client.SendAsync(bytes, SocketFlags.None);

            var received = await Client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);

            if (response == "<|ACK|>")
            {
                Console.WriteLine(
                    $"Socket client received acknowledgment: \"{response}\"");
                break;
            }


            Console.WriteLine($"Sent Message {message}");


        }

        Client.Shutdown(SocketShutdown.Both);
        Client.Close();
    }
}