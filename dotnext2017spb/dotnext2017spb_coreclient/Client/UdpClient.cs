using System;
using System.Net;

namespace DotNext
{
  public class UdpClient : IContract, IDisposable
  {
    private readonly System.Net.Sockets.UdpClient server = new System.Net.Sockets.UdpClient();
    private readonly System.Net.Sockets.UdpClient replySocket = new System.Net.Sockets.UdpClient(16001);
    private readonly IPEndPoint destination = new IPEndPoint(IPAddress.Parse(Program.ServerIP), 16000);

    public UdpClient()
    {
      
    }

    public void Dispose()
    {
      replySocket.Dispose();
      server.Dispose();
    }

    public ReplyData GetReply(InputData data)
    {
      byte[] inputBuffer = ByteArray.CreateFrom(data);
      server.SendAsync(inputBuffer, inputBuffer.Length, destination).Wait();

      var message = replySocket.ReceiveAsync().Result.Buffer;
      var replyData = message.ConvertTo<ReplyData>();
      return replyData;
    }
  }
}