using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DotNext
{
  public class RabbitMqClient : IContract, IDisposable
  {
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly AutoResetEvent replyReady = new AutoResetEvent(false);
    private ReplyData reply;

    public RabbitMqClient()
    {
      var factory = new ConnectionFactory() { HostName = Program.ServerIP };
      connection = factory.CreateConnection();
      channel = connection.CreateModel();
      replyQueueName = channel.QueueDeclare().QueueName;
      consumer = new EventingBasicConsumer(channel);
      consumer.Received += (model, ea) =>
      {
        reply = ea.Body.ConvertTo<ReplyData>();
        replyReady.Set();
      };
      channel.BasicConsume(queue: replyQueueName,
        noAck: true,
        consumer: consumer);


    }

    public ReplyData GetReply(InputData data)
    {
      reply = null;
      var corrId = Guid.NewGuid().ToString();
      var props = channel.CreateBasicProperties();
      props.ReplyTo = replyQueueName;
      props.CorrelationId = corrId;

      var messageBytes = ByteArray.CreateFrom(data);
      channel.BasicPublish(exchange: "",
        routingKey: "rpc_queue",
        basicProperties: props,
        body: messageBytes);

      replyReady.WaitOne();
      return reply;
    }

    public void Dispose()
    {
      channel.Dispose();
      connection.Dispose();
    }
  }
}