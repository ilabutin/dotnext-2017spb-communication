using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace DotNext.Benchmarks
{
  [CoreJob]
  [MemoryDiagnoser]
  public class BaseTest<T> where T : IContract, new()
  {
    protected readonly InputData inputData;
    protected readonly ReplyData expectedReply;

    public BaseTest()
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
        Console.WriteLine("Unexpected size received");
      }
      else if (!reply.Md5Hash.SequenceEqual(expectedReply.Md5Hash))
      {
        Console.WriteLine("Unexpected MD5 hash received");
      }
      else
      {
        if (BenchmarkDotNet.Environments.BenchmarkEnvironmentInfo.GetCurrent().Configuration == "DEBUG")
          Console.WriteLine("Correct reply received");
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
