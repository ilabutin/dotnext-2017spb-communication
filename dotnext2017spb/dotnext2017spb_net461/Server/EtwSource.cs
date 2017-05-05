using System.Diagnostics.Tracing;

namespace DotNext
{
  [EventSource(Name = "DotNext2017SPb-Samples-EtwSource")]
  public sealed class EtwSource : EventSource
  {
    public static readonly EtwSource Instance = new EtwSource();
    public const int RequestEventId = 1;
    public const int ReplyEventId = 2;

    [Event(RequestEventId)]
    public void SendInputData(byte[] data)
    {
      WriteEvent(RequestEventId, data);
    }

    [Event(ReplyEventId)]
    public void SendReplyData(byte[] data)
    {
      WriteEvent(ReplyEventId, data);
    }
  }
}
