using System;
using ZeroMQ;

namespace DotNext
{
  public class ZeroMqClient : IContract, IDisposable
  {
    private readonly ZContext context;
    private readonly ZSocket request;

    public ZeroMqClient()
    {
      context = new ZContext();
      request = new ZSocket(context, ZSocketType.REQ);

      request.Connect($"tcp://{Program.ServerIP}:18000");
    }
    public ReplyData GetReply(InputData data)
    {
      var inputBuf = ByteArray.CreateFrom(data);
      request.Send(new ZFrame(inputBuf));
      using (ZFrame reply = request.ReceiveFrame())
      {
        var replyData = reply.Read();
        return replyData.ConvertTo<ReplyData>();
      }
    }

    public void Dispose()
    {
      request.Dispose();
      context.Dispose();
    }
  }
}
