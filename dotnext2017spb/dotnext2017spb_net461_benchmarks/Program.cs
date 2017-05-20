using System;
using BenchmarkDotNet.Running;

namespace DotNext.Benchmarks
{
  public class TcpTest : BaseTest<TcpClient> { }
  public class EtwTest : BaseTest<EtwClient> { }
  public class MessageQueueTest : BaseTest<MessageQueueClient> { }
  public class NamedPipeTest : BaseTest<NamedPipeClient> { }
  public class RemotingTest : BaseTest<RemotingClient> { }
  public class UdpTest : BaseTest<UdpClient> { }
  public class WcfTest : BaseTest<WcfClient> { }
  public class WcfTcpTest : BaseTest<WcfTcpClient> { }
  public class WmCopyDataTest : BaseTest<WmCopyDataClient> { }
  public class ZeroMqTest : BaseTest<ZeroMqClient> { }
  public class RabbitMqTest : BaseTest<RabbitMqClient> { }
  public class MemoryMappedFileTest : BaseTest<MmfClient> { }
  public class WebApiTest : BaseTest<WebApiClient> { }
  public class NoIpcTest : BaseTest<NoIpcClient> { }
  public class NoIpcWithSerializationTest : BaseTest<NoIpcWithSerializationClient> { }

  public class NoIpcClient : IContract
  {
    public ReplyData GetReply(InputData data)
    {
      return ServerLogic.Convert(data);
    }
  }

  public class NoIpcWithSerializationClient : IContract
  {
    public ReplyData GetReply(InputData data)
    {
      var inputBuf = ByteArray.CreateFrom(data);
      var input = inputBuf.ConvertTo<InputData>();
      var reply = ServerLogic.Convert(input);
      var replyBuf = ByteArray.CreateFrom(reply);
      return replyBuf.ConvertTo<ReplyData>();
    }
  }

  class Program
  {
    public static string serverIP = "127.0.0.1";
    static void Main(string[] args)
    {
      if (args.Length > 0)
      {
        serverIP = args[0];
      }
      Console.Write("Please specify IPC client method to benchmark: ");
      string ipcMethod = Console.ReadLine();
      switch (ipcMethod)
      {
        case "noipc":
          BenchmarkRunner.Run<NoIpcTest>();
          break;
        case "noipc_s":
          BenchmarkRunner.Run<NoIpcWithSerializationTest>();
          break;
        case "wcf":
          BenchmarkRunner.Run<WcfTest>();
          break;
        case "wcftcp":
          BenchmarkRunner.Run<WcfTcpTest>();
          break;
        case "udp":
          BenchmarkRunner.Run<UdpTest>();
          break;
        case "tcp":
          BenchmarkRunner.Run<TcpTest>();
          break;
        case "remoting":
          BenchmarkRunner.Run<RemotingTest>();
          break;
        case "mq":
          BenchmarkRunner.Run<MessageQueueTest>();
          break;
        case "pipe":
          BenchmarkRunner.Run<NamedPipeTest>();
          break;
        case "mmf":
          BenchmarkRunner.Run<MemoryMappedFileTest>();
          break;
        case "etw":
          BenchmarkRunner.Run<EtwTest>();
          break;
        case "wmcopydata":
          BenchmarkRunner.Run<WmCopyDataTest>();
          break;
        case "zeromq":
          BenchmarkRunner.Run<ZeroMqTest>();
          break;
        case "rabbitmq":
          BenchmarkRunner.Run<RabbitMqTest>();
          break;
        case "webapi":
          BenchmarkRunner.Run<WebApiTest>();
          break;
        case "all":
          BenchmarkRunner.Run<WcfTest>();
          BenchmarkRunner.Run<WcfTcpTest>();
          BenchmarkRunner.Run<UdpTest>();
          BenchmarkRunner.Run<TcpTest>();
          BenchmarkRunner.Run<RemotingTest>();
          BenchmarkRunner.Run<NamedPipeTest>();
          BenchmarkRunner.Run<ZeroMqTest>();
          BenchmarkRunner.Run<RabbitMqTest>();
          break;
      }
    }
  }
}
