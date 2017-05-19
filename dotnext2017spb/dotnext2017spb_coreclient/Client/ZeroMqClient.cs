using System;
using NetMQ;
using NetMQ.Sockets;

namespace DotNext
{
  public class ZeroMqClient : IContract, IDisposable
  {
    private readonly RequestSocket request;

    public ZeroMqClient()
    {
      request = new RequestSocket($">tcp://{Program.ServerIP}:18000");
    }
    public ReplyData GetReply(InputData data)
    {
      var inputBuf = ByteArray.CreateFrom(data);
      request.SendFrame(inputBuf);
      var replyData = request.ReceiveFrameBytes();
      return replyData.ConvertTo<ReplyData>();
    }

    public void Dispose()
    {
      request.Dispose();
    }
  }
}
