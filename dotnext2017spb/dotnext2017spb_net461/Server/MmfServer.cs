using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNext
{
  public sealed class MmfServer : IServer
  {
    public static readonly string RequestReadyEventName = typeof(IContract).Name + "_request";
    public static readonly string ReplyReadyEventName = typeof(IContract).Name + "_reply";
    public static readonly string SharedFileName = typeof(IContract).Name + "_file";

    private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);

    public static EventWaitHandle OpenEvent(string name)
    {
      if (EventWaitHandle.TryOpenExisting(name, out EventWaitHandle e) == false)
      {
        e = new EventWaitHandle(false, EventResetMode.AutoReset, name);
      }
      return e;
    }

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        EventWaitHandle requestReadyEvent = OpenEvent(RequestReadyEventName);
        EventWaitHandle replyReadyEvent = OpenEvent(ReplyReadyEventName);
        
        using (requestReadyEvent)
        using (replyReadyEvent)
        using (var file = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateOrOpen(SharedFileName, Constants.MaxMessageSize))
        using (var view = file.CreateViewAccessor())
        {
          while (WaitHandle.WaitAny(new WaitHandle[] { stopEvent, requestReadyEvent }) == 1)
          {
            int inputLength = view.ReadInt32(0);
            var data = new byte[inputLength];
            view.ReadArray(4, data, 0, data.Length);
            var inputData = data.ConvertTo<InputData>();
            var replyData = ServerLogic.Convert(inputData);
            var replyBuf = ByteArray.CreateFrom(replyData);
            view.Write(0, replyBuf.Length);
            view.WriteArray(4, replyBuf, 0, replyBuf.Length);
            replyReadyEvent.Set();
          }
        }
      });
    }

    void IDisposable.Dispose()
    {
      stopEvent.Set();
      stopEvent.Dispose();
    }
  }
}
