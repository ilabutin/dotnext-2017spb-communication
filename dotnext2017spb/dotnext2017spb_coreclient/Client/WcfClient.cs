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
  public class WcfClient : ClientBase<IContract>, IContract
  {
    public WcfClient()
      : base(new BinaryHttpBinding(), new EndpointAddress("http://localhost:20000/dotnext2017spb"))
    {
    }
    public ReplyData GetFileData(InputData data)
    {
      return this.Channel.GetFileData(data);
    }
  }
}
