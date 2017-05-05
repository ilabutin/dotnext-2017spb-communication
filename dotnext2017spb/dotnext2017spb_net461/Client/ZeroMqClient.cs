using System;
using ZeroMQ;

namespace DotNext
{
  public class ZeroMqClient : IContract, IDisposable
  {
    public ReplyData GetFileData(InputData data)
    {
      using (ZContext context = new ZContext())
      using (ZSocket request = new ZSocket(context, ZSocketType.REQ))
      {
        request.Connect("tcp://127.0.0.1:18000");

        var inputBuf = ByteArray.CreateFrom(data);
        request.Send(new ZFrame(inputBuf));
        using (ZFrame reply = request.ReceiveFrame())
        {
          var replyData = reply.Read();
          return replyData.ConvertTo<ReplyData>();
        }
      }
    }

    public void Dispose()
    {
    }
  }
}
