using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace DotNext.Benchmarks
{
  //[CoreJob]
  [LegacyJitX86Job]
  //[RyuJitX64Job]
  //[MonoJob("Mono 4.8.1 x64", @"C:\Program Files\Mono\bin\mono.exe")]
  //[MonoJob("Mono 4.8.1 x86", @"C:\Program Files (x86)\Mono\bin\mono.exe")]
  //[MemoryDiagnoser]
  public class BaseTest<T> where T : IContract, new()
  {
    protected InputData inputData;
    protected ReplyData expectedReply;

    [Setup]
    public void Setup()
    {
      inputData = new InputData
      {
        Name = "fileName",
        Content = new byte[10 * 1024]
      };
      new Random().NextBytes(inputData.Content);
      expectedReply = ServerLogic.Convert(inputData);
    }

    protected void VerifyReply(ReplyData reply)
    {
      if (reply.Size != expectedReply.Size)
      {
        throw new InvalidOperationException();
      }
    }

    [Benchmark]
    public void Benchmark()
    {
      var client = new T();
      VerifyReply(client.GetFileData(inputData));
      (client as IDisposable)?.Dispose();
    }
  }
}
