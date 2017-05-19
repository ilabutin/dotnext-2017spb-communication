using System;
using System.Messaging;
using System.Threading.Tasks;

namespace DotNext
{
  public sealed class MessageQueueServer : IServer
  {
    private MessageQueue requestQueue;
    private MessageQueue replyQueue;

    void IDisposable.Dispose()
    {
      requestQueue.Close();
      requestQueue.Dispose();
      replyQueue.Close();
      replyQueue.Dispose();
    }

    public static MessageQueue CreateQueue(string name)
    {
      if (MessageQueue.Exists(name))
      {
        return new MessageQueue(name);
      }
      else
      {
        return MessageQueue.Create(name);
      }
    }

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        var requestQueueName = string.Format(".\\Private$\\{0}_requests", typeof(IContract).Name);
        var replyQueueName = string.Format("FormatName:Direct=TCP:{0}\\Private$\\{1}_replies", Program.ClientIP, typeof(IContract).Name);

        requestQueue = CreateQueue(requestQueueName);
        replyQueue = CreateQueue(replyQueueName);

        requestQueue.Formatter = new BinaryMessageFormatter();
        replyQueue.Formatter = new BinaryMessageFormatter();

        while (true)
        {
          var msg = requestQueue.Receive();
          var data = (InputData)msg.Body;
          var reply = ServerLogic.Convert(data);
          replyQueue.Send(reply);
        }
      });
    }
  }
}