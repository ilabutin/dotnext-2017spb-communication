using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;

namespace DotNext
{
  public sealed class RemotingServer : IServer
  {
    private class InternalRemotingServer : MarshalByRefObject, IContract
    {
      public ReplyData GetReply(InputData data)
      {
        return ServerLogic.Convert(data);
      }
    }

    private readonly ManualResetEvent killer = new ManualResetEvent(false);

    private static readonly IServerChannelSinkProvider serverSinkProvider =
      new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        //var properties = new Hashtable { ["portName"] = typeof(IContract).Name };
        RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
        var channel = new TcpChannel(21000);

        try
        {
          ChannelServices.RegisterChannel(channel, true);
        }
        catch
        {
          //might be already registered, ignore it
        }

        RemotingConfiguration.RegisterWellKnownServiceType(typeof(InternalRemotingServer), "remoteObject", WellKnownObjectMode.Singleton);

        //var remoteObject = new RemoteObject(new InternalRemotingServer(this));

        //RemotingServices.Marshal(remoteObject, typeof(RemoteObject).Name + ".rem");

        killer.WaitOne();

        //RemotingServices.Disconnect(remoteObject);

        try
        {
          ChannelServices.UnregisterChannel(channel);
        }
        catch
        {
        }
      });
    }
    
    void IDisposable.Dispose()
    {
      killer.Set();
      killer.Dispose();
    }
  }
}
