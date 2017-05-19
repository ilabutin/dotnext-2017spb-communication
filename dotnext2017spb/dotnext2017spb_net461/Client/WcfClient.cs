using System.ServiceModel;
using System.ServiceModel.Channels;

namespace DotNext
{
  public class BinaryHttpBinding : CustomBinding
  {
    public BinaryHttpBinding() : this(null)
    {
      
    }
    public BinaryHttpBinding(long? maxReceivedMessageSize)
    {
      Elements.Add(new BinaryMessageEncodingBindingElement());
      var transportBinding = new HttpTransportBindingElement();
      if (maxReceivedMessageSize.HasValue)
      {
        transportBinding.MaxReceivedMessageSize = maxReceivedMessageSize.Value;
      }
      Elements.Add(transportBinding);
    }
  }
  public class BinaryTcpBinding : CustomBinding
  {
    public BinaryTcpBinding() : this(null)
    {

    }
    public BinaryTcpBinding(long? maxReceivedMessageSize)
    {
      Elements.Add(new BinaryMessageEncodingBindingElement());
      var transportBinding = new TcpTransportBindingElement();
      if (maxReceivedMessageSize.HasValue)
      {
        transportBinding.MaxReceivedMessageSize = maxReceivedMessageSize.Value;
      }
      Elements.Add(transportBinding);
    }
  }
  public class WcfClient : ClientBase<IContract>, IContract
  {
    public WcfClient()
      : base(new BinaryHttpBinding(), new EndpointAddress($"http://{Program.ServerIP}:20000/dotnext2017spb"))
    {
    }
    public ReplyData GetReply(InputData data)
    {
      return this.Channel.GetReply(data);
    }
  }
  public class WcfTcpClient : ClientBase<IContract>, IContract
  {
    public WcfTcpClient()
      : base(new BinaryTcpBinding(), new EndpointAddress($"net.tcp://{Program.ServerIP}:20001/dotnext2017spb"))
    {
    }
    public ReplyData GetReply(InputData data)
    {
      return this.Channel.GetReply(data);
    }
  }
}
