using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace DotNext
{
  public sealed class NamedPipeServer : IServer
  {
    private readonly NamedPipeServerStream server =
      new NamedPipeServerStream(
        typeof(IContract).Name + "_pipe",
        PipeDirection.InOut,
        maxNumberOfServerInstances:1,
        transmissionMode:PipeTransmissionMode.Byte,
        options:PipeOptions.Asynchronous);

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        while (true)
        {
          server.WaitForConnection();

          while (true)
          {
            var inputData = server.Receive<InputData>();
            if (inputData == null)
            {
              break;
            }
            var reply = ServerLogic.Convert(inputData);
            server.Send(reply);

            server.WaitForPipeDrain();
          }

          server.Disconnect();
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
