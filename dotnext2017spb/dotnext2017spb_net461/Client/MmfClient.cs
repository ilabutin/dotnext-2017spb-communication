using System;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace DotNext
{
  public class MmfClient : IContract, IDisposable
  {
    private readonly EventWaitHandle requestReadyEvent;
    private readonly EventWaitHandle replyReadyEvent;
    private readonly MemoryMappedFile file;
    private readonly MemoryMappedViewAccessor view;

    public MmfClient()
    {
      requestReadyEvent = MmfServer.OpenEvent(MmfServer.RequestReadyEventName);
      replyReadyEvent = MmfServer.OpenEvent(MmfServer.ReplyReadyEventName);
      file = MemoryMappedFile.CreateOrOpen(MmfServer.SharedFileName, Constants.MaxMessageSize);
      view = file.CreateViewAccessor();
    }
    public ReplyData GetReply(InputData data)
    {
      var inputBuf = ByteArray.CreateFrom(data);
      view.Write(0, inputBuf.Length);
      view.WriteArray(4, inputBuf, 0, inputBuf.Length);
      requestReadyEvent.Set();
      replyReadyEvent.WaitOne();
      int replyLength = view.ReadInt32(0);
      var buf = new byte[replyLength];
      view.ReadArray(4, buf, 0, buf.Length);
      var replyData = buf.ConvertTo<ReplyData>();
      return replyData;
    }

    public void Dispose()
    {
      view.Dispose();
      file.Dispose();
      requestReadyEvent?.Dispose();
      replyReadyEvent?.Dispose();
    }
  }
}
