using System;
using System.Diagnostics.Tracing;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;

namespace DotNext
{
  public class EtwClient : IContract, IDisposable
  {
    private readonly AutoResetEvent replyReceived = new AutoResetEvent(false);
    private ReplyData replyData;
    private readonly TraceEventSession etwSession;

    public EtwClient()
    {
      etwSession = new TraceEventSession("ReplySession");
      etwSession.Source.Dynamic.All += HandleEtwEvent;
      var eventSourceProviderGuid = TraceEventProviders.GetEventSourceGuidFromName(EventSource.GetName(typeof(EtwSource)));
      etwSession.EnableProvider(eventSourceProviderGuid);
      Task.Factory.StartNew(() => etwSession.Source.Process());
    }

    public void Dispose()
    {
      etwSession.Stop();
      etwSession.Dispose();
    }

    public ReplyData GetReply(InputData data)
    {
      var inputBuf = ByteArray.CreateFrom(data);
      EtwSource.Instance.SendInputData(inputBuf);
      replyReceived.WaitOne();
      return replyData;
    }

    private void HandleEtwEvent(TraceEvent eventData)
    {
      if ((int)eventData.ID == EtwSource.ReplyEventId)
      {
        var data = eventData.PayloadByName("data") as byte[];
        replyData = data.ConvertTo<ReplyData>();
        replyReceived.Set();
      }
    }
  }
}
