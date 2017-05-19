using System;
using System.IO.Pipes;

namespace DotNext
{
  public class NamedPipeClient : IContract, IDisposable
  {
    private readonly NamedPipeClientStream client;

    public NamedPipeClient()
    {
      client = new NamedPipeClientStream(
        Program.ServerIP,
        typeof(IContract).Name + "_pipe",
        PipeDirection.InOut);
      client.Connect();
    }
    public ReplyData GetReply(InputData data)
    {
      client.Send(data);
      client.WaitForPipeDrain();
      var reply = client.Receive<ReplyData>();
      return reply;
    }

    public void Dispose()
    {
      client.Dispose();
    }
  }
}
