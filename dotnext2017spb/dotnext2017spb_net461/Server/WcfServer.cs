using System;
using System.ServiceModel;

namespace DotNext
{
  public class WcfServer : IServer
  {
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class InternalWcfServer : IContract
    {
      public ReplyData GetFileData(InputData data)
      {
        return ServerLogic.Convert(data);
      }
    }
    private ServiceHost host;

    public void Dispose()
    {
      host.Close();
    }

    public void Start()
    {
      host = new ServiceHost(typeof(InternalWcfServer));
      host.AddServiceEndpoint(typeof(IContract),
        new BinaryHttpBinding(200 * 1024 * 1024),
        new Uri("http://localhost:20000/dotnext2017spb"));
      host.Open();
    }
  }
}
