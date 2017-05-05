using System;
using System.Net;

namespace DotNext
{
  public class TcpClient : IContract, IDisposable
  {
    private readonly System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();

    public TcpClient()
    {
      client.Connect(new IPEndPoint(IPAddress.Loopback, 17000));
    }

    public ReplyData GetFileData(InputData data)
    {
      var networkStream = client.GetStream();
      networkStream.Send(data);
      var reply = networkStream.Receive<ReplyData>();
      return reply;
    }

    public void Dispose()
    {
      client.Close();
    }
  }
}
