using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DotNext
{
  public class TcpServer : IServer
  {
    private readonly TcpListener listener = new TcpListener(IPAddress.Any, 17000);

    public void Dispose()
    {
      listener.Stop();
    }

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        listener.Start();

        while (true)
        {
          var tcpClient = listener.AcceptTcpClient();
          Console.WriteLine("Connected");
          var networkStream = tcpClient.GetStream();
          try
          {
            while (true)
            {
              var inputData = networkStream.Receive<InputData>();
              if (inputData == null)
              {
                Console.WriteLine("Disconnected");
                break;
              }
              var reply = ServerLogic.Convert(inputData);
              networkStream.Send(reply);
            }
          }
          catch (Exception e)
          {
            Console.WriteLine("Exception: {0}", e);
          }
          finally
          {
            tcpClient.Dispose();
          }
        }
      });
    }
  }
}
