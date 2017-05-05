using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Session;

namespace DotNext
{
  public sealed class EtwServer : IServer
  {
    private TraceEventSession etwSession;
    private void HandleEtwEvent(TraceEvent eventData)
    {
      if ((int)eventData.ID == EtwSource.RequestEventId)
      {
        var data = eventData.PayloadByName("data") as byte[];
        var inputData = data.ConvertTo<InputData>();
        var reply = ServerLogic.Convert(inputData);
        var replyBuf = ByteArray.CreateFrom(reply);
        EtwSource.Instance.SendReplyData(replyBuf);
      }
    }

    public void Start()
    {
      etwSession = new TraceEventSession("RequestSession");
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
  }
}
