using System;
using System.Linq;

namespace DotNext
{
  internal class Program
  {
//    public static string ServerIP = "127.0.0.1";
//    public static string ClientIP = "127.0.0.1";
        public static string ServerIP = "192.168.54.253";
        public static string ClientIP = "192.168.55.37";
        //public static string ServerIP = "192.168.54.253";
//        public static string ClientIP = "192.168.55.26";

    private static InputData inputData;
    private static ReplyData expectedReply;

    private static void VerifyReply(ReplyData reply)
    {
      if (reply.Size != expectedReply.Size)
      {
        throw new InvalidOperationException();
      }
    }

    private static void Test<T>() where T : IContract, new()
    {
      var client = new T();
      for (int i = 0; i < 100000; i++)
      {
        VerifyReply(client.GetReply(inputData));
      }
      (client as IDisposable)?.Dispose();
    }

    private static void Main(string[] args)
    {
      inputData = new InputData
      {
        Content = new byte[1]
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
        case "pipe":
          Test<NamedPipeClient>();
          break;
        case "mmf":
          Test<MmfClient>();
          break;
        case "zeromq":
          Test<ZeroMqClient>();
          break;
        case "rabbitmq":
          Test<RabbitMqClient>();
          break;
      }
      Console.WriteLine("Client test completed.");
      Console.ReadLine();
    }
  }
}
