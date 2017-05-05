using System;
using System.Messaging;
using System.Threading.Tasks;

namespace DotNext
{
  public sealed class MessageQueueServer : IServer
  {
    private System.Messaging.MessageQueue requestQueue;
    private System.Messaging.MessageQueue replyQueue;

    void IDisposable.Dispose()
    {
      requestQueue.Close();
      requestQueue.Dispose();
      replyQueue.Close();
      replyQueue.Dispose();
    }

    public static System.Messaging.MessageQueue CreateQueue(string name)
    {
      if (System.Messaging.MessageQueue.Exists(name))
      {
        return new System.Messaging.MessageQueue(name);
      }
      else
      {
        return System.Messaging.MessageQueue.Create(name);
      }
    }

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        var requestQueueName = string.Format(".\\Private$\\{0}_requests", typeof(IContract).Name);
        var replyQueueName = string.Format(".\\Private$\\{0}_replies", typeof(IContract).Name);

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