using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ZeroMQ;

namespace DotNext
{
  public class RabbitMqServer : IServer
  {
    private ConnectionFactory factory;
    private IConnection connection;
    private IModel channel;

    public void Dispose()
    {
      channel.Dispose();
      connection.Dispose();
    }

    public void Start()
    {
      factory = new ConnectionFactory() { HostName = "localhost" };
      connection = factory.CreateConnection();
      channel = connection.CreateModel();

      channel.QueueDeclare(queue: "rpc_queue", durable: false,
        exclusive: false, autoDelete: false, arguments: null);
      channel.BasicQos(0, 1, false);
      var consumer = new EventingBasicConsumer(channel);
      channel.BasicConsume(queue: "rpc_queue",
        noAck: false, consumer: consumer);

      consumer.Received += (model, ea) =>
      {
        byte[] response = null;

        var body = ea.Body;
        var props = ea.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        try
        {
          var inputData = body.ConvertTo<InputData>();
          var replyData = ServerLogic.Convert(inputData);
          response = ByteArray.CreateFrom(replyData);
        }
        catch (Exception e)
        {
          Console.WriteLine("Error: {0}", e);
          response = new byte[0];
        }
        finally
        {
          channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
            basicProperties: replyProps, body: response);
          channel.BasicAck(deliveryTag: ea.DeliveryTag,
            multiple: false);
        }
      };
    }
  }
}