using System;
using System.Messaging;

namespace DotNext
{
  public class MessageQueueClient : IContract, IDisposable
  {
    private readonly MessageQueue requestQueue;
    private readonly MessageQueue replyQueue;

    public MessageQueueClient()
    {
      var requestQueueName = string.Format("FormatName:Direct=TCP:{0}\\Private$\\{1}_requests", Program.ServerIP, typeof(IContract).Name);
      var replyQueueName = string.Format(".\\Private$\\{0}_replies", typeof(IContract).Name);

      requestQueue = MessageQueueServer.CreateQueue(requestQueueName);
      replyQueue = MessageQueueServer.CreateQueue(replyQueueName);

      requestQueue.Formatter = new BinaryMessageFormatter();
      replyQueue.Formatter = new BinaryMessageFormatter();
    }
    public ReplyData GetReply(InputData data)
    {
      requestQueue.Send(data);
      var replyMsg = replyQueue.Receive();
      var replyData = (ReplyData)replyMsg?.Body;
      return replyData;
    }

    public void Dispose()
    {
      requestQueue?.Dispose();
      replyQueue?.Dispose();
    }
  }
}
