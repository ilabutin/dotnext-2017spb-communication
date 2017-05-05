using System.Messaging;

namespace DotNext
{
  public class MessageQueueClient : IContract
  {
    public ReplyData GetFileData(InputData data)
    {
      var requestQueueName = string.Format(".\\Private$\\{0}_requests", typeof(IContract).Name);
      var replyQueueName = string.Format(".\\Private$\\{0}_replies", typeof(IContract).Name);

      var requestQueue = MessageQueueServer.CreateQueue(requestQueueName);
      var replyQueue = MessageQueueServer.CreateQueue(replyQueueName);

      requestQueue.Formatter = new BinaryMessageFormatter();
      replyQueue.Formatter = new BinaryMessageFormatter();

      using (requestQueue)
      {
        requestQueue.Send(data);
      }

      var replyMsg = replyQueue.Receive();
      var replyData = (ReplyData)replyMsg?.Body;
      return replyData;
    }
  }
}
