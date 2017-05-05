using System;
using System.IO.Pipes;

namespace DotNext
{
  public class NamedPipeClient : IContract, IDisposable
  {
    public ReplyData GetFileData(InputData data)
    {
      using (var client = new NamedPipeClientStream(".", typeof(IContract).Name + "_pipe", PipeDirection.InOut))
      {
        client.Connect();

        client.Send(data);
        var reply = client.Receive<ReplyData>();
        return reply;
      }
    }

    public void Dispose()
    {
    }
  }
}
