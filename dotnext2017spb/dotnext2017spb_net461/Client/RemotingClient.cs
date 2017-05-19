using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace DotNext
{
  public class RemotingClient : IContract, IDisposable
  {
    private static readonly IServerChannelSinkProvider ServerSinkProvider =
      new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

    private readonly IContract svc;
    private readonly TcpChannel channel;

    public RemotingClient()
    {
//      var properties = new Hashtable
//      {
//        ["portName"] = Guid.NewGuid().ToString(),
//        ["exclusiveAddressUse"] = false
//      };
//      channel = new IpcChannel(properties, null, ServerSinkProvider);
      channel = new TcpChannel();

      try
      {
        ChannelServices.RegisterChannel(channel, true);
      }
      catch
      {
        //the channel might be already registered, so ignore it
      }


      //var uri = string.Format("tcp://{0}/{1}.rem", typeof(IContract).Name, typeof(RemoteObject).Name);
      svc = (IContract)Activator.GetObject(typeof(RemoteObject), $"tcp://{Program.ServerIP}:21000/remoteObject");
    }

    public ReplyData GetReply(InputData data)
    {
      return svc.GetReply(data);
    }

    public void Dispose()
    {
      try
      {
        ChannelServices.UnregisterChannel(channel);
      }
      catch
      {
      }
    }
  }
}
