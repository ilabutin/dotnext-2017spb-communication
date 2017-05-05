using System;
using System.Net;

namespace DotNext
{
  public class UdpClient : IContract, IDisposable
  {
    private readonly System.Net.Sockets.UdpClient server = new System.Net.Sockets.UdpClient();
    private readonly System.Net.Sockets.UdpClient replySocket = new System.Net.Sockets.UdpClient(16001);

    public void Dispose()
    {
      replySocket.Dispose();
      server.Dispose();
    }

    public ReplyData GetFileData(InputData data)
    {
      byte[] inputBuffer = ByteArray.CreateFrom(data);
      server.SendAsync(inputBuffer, inputBuffer.Length, "", 16000).Wait();

      var peerAddress = new IPEndPoint(IPAddress.Any, 0);
      var message = replySocket.ReceiveAsync().Result.Buffer;
      var replyData = message.ConvertTo<ReplyData>();
      return replyData;
    }
  }
}