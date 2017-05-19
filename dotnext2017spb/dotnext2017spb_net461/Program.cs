using System;
using System.Linq;

namespace DotNext
{
  internal class Program
  {
    public static string ServerIP = "127.0.0.1";
    public static string ClientIP = "127.0.0.1";
    //    public static string ServerIP = "192.168.54.253";
    //    public static string ClientIP = "192.168.55.37";
//        public static string ServerIP = "192.168.54.253";
//        public static string ClientIP = "192.168.55.26";
    private static void Main(string[] args)
    {
      string ipcMethod;
      if (args.Length > 0)
      {
        ipcMethod = args[0];
      }
      else
      {
        Console.Write("Please specify IPC server method to use: ");
        ipcMethod = Console.ReadLine();
      }
      switch (ipcMethod)
      {
        case "wcf":
          using (var server = new WcfServer())
          {
            server.Start();
            Console.WriteLine("WCF Server started");
            Console.ReadLine();
          }
          break;
        case "udp":
          using (var server = new UdpServer())
          {
            server.Start();
            Console.WriteLine("UDP Server started");
            Console.ReadLine();
          }
          break;
        case "tcp":
          using (var server = new TcpServer())
          {
            server.Start();
            Console.WriteLine("TCP Server started");
            Console.ReadLine();
          }
          break;
        case "remoting":
          using (var server = new RemotingServer())
          {
            server.Start();
            Console.WriteLine("Remoting Server started");
            Console.ReadLine();
          }
          break;
        case "mq":
          using (var server = new MessageQueueServer())
          {
            server.Start();
            Console.WriteLine("MessageQueue Server started");
            Console.ReadLine();
          }
          break;
        case "pipe":
          using (var server = new NamedPipeServer())
          {
            server.Start();
            Console.WriteLine("NamedPipes Server started");
            Console.ReadLine();
          }
          break;
        case "mmf":
          using (var server = new MmfServer())
          {
            server.Start();
            Console.WriteLine("MemoryMappedFile Server started");
            Console.ReadLine();
          }
          break;
        case "etw":
          using (var server = new EtwServer())
          {
            server.Start();
            Console.WriteLine("ETW Server started");
            Console.ReadLine();
          }
          break;
        case "wmcopydata":
          using (var server = new WmCopyDataServer())
          {
            server.Start();
            Console.WriteLine("WMCopyData Server started");
            Console.ReadLine();
          }
          break;
        case "zeromq":
          using (var server = new ZeroMqServer())
          {
            server.Start();
            Console.WriteLine("ZeroMQ Server started");
            Console.ReadLine();
          }
          break;
        case "rabbitmq":
          using (var server = new RabbitMqServer())
          {
            server.Start();
            Console.WriteLine("RabbitMQ Server started");
            Console.ReadLine();
          }
          break;
        case "all":
          using (var wcfServer = new WcfServer())
          using (var udpServer = new UdpServer())
          using (var tcpServer = new TcpServer())
          using (var remotingServer = new RemotingServer())
          using (var mqServer = new MessageQueueServer())
          using (var pipeServer = new NamedPipeServer())
          using (var mmfServer = new MmfServer())
          using (var etwServer = new EtwServer())
          using (var wmServer = new WmCopyDataServer())
          using (var zmqServer = new ZeroMqServer())
          {
            wcfServer.Start();
            udpServer.Start();
            tcpServer.Start();
            remotingServer.Start();
            mqServer.Start();
            pipeServer.Start();
            mmfServer.Start();
            etwServer.Start();
            wmServer.Start();
            zmqServer.Start();
            Console.WriteLine("All servers started");
            Console.ReadLine();
          }
          break;
      }
    }
  }
}
