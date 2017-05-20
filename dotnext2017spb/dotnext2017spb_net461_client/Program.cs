using System;

namespace DotNext
{
  internal class Program
  {
    private static InputData inputData;
    private static ReplyData expectedReply;

    private static void VerifyReply(ReplyData reply)
    {
      if (reply.Size != expectedReply.Size)
      {
        throw new InvalidOperationException();
      }
    }

//    private static void Test<T>() where T : IContract, new()
//    {
//      var client = new T();
//      for (int i = 0; i < 100; i++)
//      {
//        VerifyReply(client.GetReply(inputData));
//      }
//      (client as IDisposable)?.Dispose();
//    }

    private static void Test<T>() where T : IContract, new()
    {
      for (int j = 0; j < 1; j++)
      {
        var client = new T();
        for (int i = 0; i < 100; i++)
        {
          VerifyReply(client.GetReply(inputData));
        }
        (client as IDisposable)?.Dispose();
        Console.WriteLine("Client {0} done", j);
      }
    }

    private static void Main(string[] args)
    {
      inputData = new InputData
      {
        Content = new byte[100 * 1024]
      };
      new Random().NextBytes(inputData.Content);
      expectedReply = ServerLogic.Convert(inputData);

      string ipcMethod;
      if (args.Length > 0)
      {
        ipcMethod = args[0];
      }
      else
      {
        Console.Write("Please specify IPC client method to use: ");
        ipcMethod = Console.ReadLine();
      }
      switch (ipcMethod)
      {
        case "noipc":
          var inputBuf = ByteArray.CreateFrom(inputData);
          var input = inputBuf.ConvertTo<InputData>();
          var reply = ServerLogic.Convert(input);
          var replyBuf = ByteArray.CreateFrom(reply);
          VerifyReply(replyBuf.ConvertTo<ReplyData>());
          break;
        case "wcf":
          Test<WcfClient>();
          break;
        case "wcftcp":
          Test<WcfTcpClient>();
          break;
        case "udp":
          Test<UdpClient>();
          break;
        case "tcp":
          Test<TcpClient>();
          break;
        case "remoting":
          Test<RemotingClient>();
          break;
        case "mq":
          Test<MessageQueueClient>();
          break;
        case "pipe":
          Test<NamedPipeClient>();
          break;
        case "mmf":
          Test<MmfClient>();
          break;
        case "etw":
          Test<EtwClient>();
          break;
        case "wmcopydata":
          Test<WmCopyDataClient>();
          break;
        case "zeromq":
          Test<ZeroMqClient>();
          break;
        case "rabbitmq":
          Test<RabbitMqClient>();
          break;
        case "webapi":
          Test<WebApiClient>();
          break;
      }
      Console.WriteLine("Client test completed.");
      Console.ReadLine();
    }
  }
}
