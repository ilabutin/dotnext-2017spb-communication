using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace DotNext
{
  public sealed class NamedPipeServer : IServer
  {
    private readonly NamedPipeServerStream server = new NamedPipeServerStream(typeof(IContract).Name + "_pipe", PipeDirection.InOut, 1);

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        while (true)
        {
          server.WaitForConnection();

          var inputData = server.Receive<InputData>();
          var reply = ServerLogic.Convert(inputData);
          server.Send(reply);

          server.WaitForPipeDrain();
          if (server.IsConnected)
          {
            server.Disconnect();
          }
        }
      });
    }

    void IDisposable.Dispose()
    {
      if (server.IsConnected)
      {
        server.Disconnect();
      }
      server.Dispose();
    }
  }
}
