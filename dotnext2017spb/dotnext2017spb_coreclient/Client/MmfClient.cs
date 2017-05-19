using System;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace DotNext
{
  public class MmfClient : IContract, IDisposable
  {
    public static readonly string RequestReadyEventName = typeof(IContract).Name + "_request";
    public static readonly string ReplyReadyEventName = typeof(IContract).Name + "_reply";
    public static readonly string SharedFileName = typeof(IContract).Name + "_file";

    public static EventWaitHandle OpenEvent(string name)
    {
      if (EventWaitHandle.TryOpenExisting(name, out EventWaitHandle e) == false)
      {
        e = new EventWaitHandle(false, EventResetMode.AutoReset, name);
      }
      return e;
    }

    private readonly EventWaitHandle requestReadyEvent;
    private readonly EventWaitHandle replyReadyEvent;
    private readonly MemoryMappedFile file;
    private readonly MemoryMappedViewStream stream;

    public MmfClient()
    {
      requestReadyEvent = OpenEvent(RequestReadyEventName);
      replyReadyEvent = OpenEvent(ReplyReadyEventName);
      file = MemoryMappedFile.CreateOrOpen(SharedFileName, Constants.MaxMessageSize);
      stream = file.CreateViewStream();
    }

    public ReplyData GetReply(InputData data)
    {
      stream.Position = 0;
      stream.Send(data);
      requestReadyEvent.Set();

      replyReadyEvent.WaitOne();
      stream.Position = 0;
      var replyData = stream.Receive<ReplyData>();
      return replyData;
    }

    public void Dispose()
    {
      requestReadyEvent?.Dispose();
      replyReadyEvent?.Dispose();
      stream.Dispose();
      file?.Dispose();
    }
  }
}
