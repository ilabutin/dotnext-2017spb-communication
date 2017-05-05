using System.Threading;

namespace DotNext
{
  public class MmfClient : IContract
  {
    public ReplyData GetFileData(InputData data)
    {
      EventWaitHandle requestReadyEvent = MmfServer.OpenEvent(MmfServer.RequestReadyEventName);
      EventWaitHandle replyReadyEvent = MmfServer.OpenEvent(MmfServer.ReplyReadyEventName);

      using (requestReadyEvent)
      using (replyReadyEvent)
      using (var file = System.IO.MemoryMappedFiles.MemoryMappedFile.CreateOrOpen(MmfServer.SharedFileName, Constants.MaxMessageSize))
      using (var view = file.CreateViewAccessor())
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
    }
  }
}
