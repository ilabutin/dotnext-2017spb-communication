using System;
using NetMQ;
using NetMQ.Sockets;

namespace DotNext
{
  public class ZeroMqClient : IContract, IDisposable
  {
    public ReplyData GetFileData(InputData data)
    {
      using (var request = new RequestSocket(">tcp://127.0.0.1:18000"))
      {
        var inputBuf = ByteArray.CreateFrom(data);
        request.SendFrame(inputBuf);
        var replyData = request.ReceiveFrameBytes();
        return replyData.ConvertTo<ReplyData>();
      }
    }

    public void Dispose()
    {
    }
  }
}
