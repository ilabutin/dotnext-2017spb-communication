using System;

namespace DotNext
{
  public interface IServer : IDisposable
  {
    void Start();
  }

  public static class ServerLogic
  {
    public static ReplyData Convert(InputData data)
    {
      var reply = new ReplyData
      {
        Size = data.Content.LongLength
      };
      return reply;
    }
  }
}
