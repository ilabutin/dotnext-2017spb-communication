using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;

namespace DotNext.Benchmarks
{
  public class Config : ManualConfig
  {
    public Config()
    {
      // You can add custom tags per each method using Columns
      Add(StatisticColumn.OperationsPerSecond);

      // CLR x64
      Add(new Job("CLR", EnvMode.RyuJitX64)
      {
        Run = { WarmupCount = 5, TargetCount = 10 },
        Env = { Runtime = Runtime.Clr }
      });

      // Mono x64
      Add(new Job("Mono", EnvMode.Mono)
      {
        Run = { WarmupCount = 5, TargetCount = 10 },
        Env = { Runtime = (Runtime)new MonoRuntime("Mono", @"C:\Program Files\Mono\bin\mono.exe") }
      });
    }
  }

  //[CoreJob]
  //[LegacyJitX86Job]
  //[RyuJitX64Job]
  //[MonoJob("Mono 5.0.0 x64", @"C:\Program Files\Mono\bin\mono.exe")]
  //[MonoJob("Mono 4.8.1 x86", @"C:\Program Files (x86)\Mono\bin\mono.exe")]
  //[MemoryDiagnoser]
  [Config(typeof(Config))]
  public class BaseTest<T> where T : IContract, new()
  {
    private InputData inputData;
    private ReplyData expectedReply;
    private InputData middleInputData;
    private ReplyData middleExpectedReply;
    private InputData largeInputData;
    private ReplyData largeExpectedReply;
    private InputData messageInputData;
    private ReplyData messageExpectedReply;
    private T client;

    [Setup]
    public void Setup()
    {
      inputData = new InputData
      {
        Content = new byte[1]
      };
      new Random().NextBytes(inputData.Content);

      middleInputData = new InputData
      {
        Content = new byte[10 * 1024]
      };
      new Random().NextBytes(middleInputData.Content);

      largeInputData = new InputData
      {
        Content = new byte[100 * 1024]
      };
      new Random().NextBytes(largeInputData.Content);

      messageInputData = new InputData
      {
        Content = new byte[1024]
      };
      new Random().NextBytes(middleInputData.Content);

      expectedReply = ServerLogic.Convert(inputData);
      middleExpectedReply = ServerLogic.Convert(middleInputData);
      largeExpectedReply = ServerLogic.Convert(largeInputData);
      messageExpectedReply = ServerLogic.Convert(messageInputData);

      client = new T();
    }

    [Cleanup]
    public void Cleanup()
    {
      (client as IDisposable)?.Dispose();
    }

    protected void VerifyReply(ReplyData reply, ReplyData expected)
    {
      if (reply.Size != expected.Size)
      {
        throw new InvalidOperationException();
      }
    }

    [Benchmark]
    public void LatencyBenchmark()
    {
      VerifyReply(client.GetReply(inputData), expectedReply);
    }

    [Benchmark]
    public void Message_1K_ThroughputBenchmark()
    {
      VerifyReply(client.GetReply(messageInputData), messageExpectedReply);
    }

    [Benchmark]
    public void Message_10K_ThroughputBenchmark()
    {
      VerifyReply(client.GetReply(middleInputData), middleExpectedReply);
    }

    [Benchmark]
    public void Message_100K_ThroughputBenchmark()
    {
      VerifyReply(client.GetReply(largeInputData), largeExpectedReply);
    }

    //[Benchmark]
    public void ThroughputLarge40MbBenchmark()
    {
      for (int i = 0; i < 20; i++)
      {
        VerifyReply(client.GetReply(largeInputData), largeExpectedReply);
      }
    }

  }
}
