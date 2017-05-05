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
          var networkStream = tcpClient.GetStream();
          var inputData = networkStream.Receive<InputData>();
          var reply = ServerLogic.Convert(inputData);
          networkStream.Send(reply);
        }
      });
    }
  }
}
