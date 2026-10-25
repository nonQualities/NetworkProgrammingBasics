using System.Net;
using System.Net.Sockets;
using System.Text;

public class TcpCliSim
{
    const string EOM = "<|EOM|>";
    const string END = "END";
    const int PORT = 8080;

    public static async Task Main()
    {

        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint iPEndPoint = new(iPAddress, PORT);

            using Socket client = new(
                iPEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            await client.ConnectAsync(iPEndPoint);
            Console.WriteLine($"[INFO]: Client Connected{iPEndPoint}");
            Console.WriteLine("--------------------");

            while (true)
            {
                
                Console.WriteLine("[PROMPT]:Enter Your Message\n >>");
                var m = Console.ReadLine();
                var msg = m + EOM;
                var byt = Encoding.UTF8.GetBytes(msg);
                var bfr = new byte[2048];

                await client.SendAsync(byt, SocketFlags.None);
                var rec = await client.ReceiveAsync(bfr, SocketFlags.None);
                var res = Encoding.UTF8.GetString(bfr, 0, rec);

                if (res == ("END"+EOM))
                {
                    Console.WriteLine($"[CTRL]: ENDING COMMUNICATION. RECIEVED: {res}");
                    break;
                }

                Console.WriteLine($"[MSG]: {res}");
            }

            client.Shutdown(SocketShutdown.Both); 
            client.Close();
    }
 } 