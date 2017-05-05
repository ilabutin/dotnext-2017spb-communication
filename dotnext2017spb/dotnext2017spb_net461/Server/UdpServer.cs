using System.Net;
using System.Threading.Tasks;

namespace DotNext
{
  public class UdpServer : IServer
  {
    private readonly System.Net.Sockets.UdpClient server = new System.Net.Sockets.UdpClient(16000);
    private readonly System.Net.Sockets.UdpClient replySocket = new System.Net.Sockets.UdpClient();

    public void Dispose()
    {
      server.Close();
      replySocket.Close();
    }

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        var peerAddress = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
          var message = server.Receive(ref peerAddress);
          var inputData = message.ConvertTo<InputData>();
          var reply = ServerLogic.Convert(inputData);
          var replyBuffer = ByteArray.CreateFrom(reply);
          replySocket.Connect("", 16001);
          replySocket.Send(replyBuffer, replyBuffer.Length);
        }
      });
    }
  }
}
