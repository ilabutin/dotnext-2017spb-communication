using System.Threading.Tasks;
using ZeroMQ;

namespace DotNext
{
  public class ZeroMqServer : IServer
  {
    private ZContext context;
    private ZSocket responder;
    public void Dispose()
    {
      responder.Dispose();
      context.Dispose();
    }

    public void Start()
    {
      context = new ZContext();
      responder = new ZSocket(context, ZSocketType.REP);
      responder.Bind("tcp://*:18000");
      Task.Factory.StartNew(() =>
      {
        while (true)
        {
          // Receive
          using (ZFrame request = responder.ReceiveFrame())
          {
            byte[] inputBuf = request.Read();
            var inputData = inputBuf.ConvertTo<InputData>();
            var replyData = ServerLogic.Convert(inputData);
            byte[] replyBuf = ByteArray.CreateFrom(replyData);
            responder.Send(new ZFrame(replyBuf));
          }
        }
      });
    }
  }
}
