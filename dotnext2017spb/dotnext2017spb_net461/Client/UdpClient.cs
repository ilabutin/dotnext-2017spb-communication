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
      replySocket.Close();
      server.Close();
    }

    public ReplyData GetFileData(InputData data)
    {
      server.Connect("", 16000);
      byte[] inputBuffer = ByteArray.CreateFrom(data);
      server.Send(inputBuffer, inputBuffer.Length);

      var peerAddress = new IPEndPoint(IPAddress.Any, 0);
      var message = replySocket.Receive(ref peerAddress);
      var replyData = message.ConvertTo<ReplyData>();
      return replyData;
    }
  }
}