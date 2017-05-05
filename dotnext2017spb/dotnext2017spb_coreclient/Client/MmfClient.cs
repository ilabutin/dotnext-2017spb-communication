using System.IO.MemoryMappedFiles;
using System.Threading;

namespace DotNext
{
  public class MmfClient : IContract
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

    public ReplyData GetFileData(InputData data)
    {
      EventWaitHandle requestReadyEvent = OpenEvent(RequestReadyEventName);
      EventWaitHandle replyReadyEvent = OpenEvent(ReplyReadyEventName);

      using (requestReadyEvent)
      using (replyReadyEvent)
      using (var file = MemoryMappedFile.CreateOrOpen(SharedFileName, Constants.MaxMessageSize))
      {
        using (MemoryMappedViewStream stream = file.CreateViewStream())
        {
          stream.Send(data);
          requestReadyEvent.Set();
        }

        replyReadyEvent.WaitOne();
        using (MemoryMappedViewStream stream = file.CreateViewStream())
        {
          var replyData = stream.Receive<ReplyData>();
          return replyData;
        }
      }
    }
  }
}
