using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;

namespace DotNext
{
  public class RemotingClient : IContract
  {
    private static readonly IServerChannelSinkProvider ServerSinkProvider =
      new BinaryServerFormatterSinkProvider { TypeFilterLevel = TypeFilterLevel.Full };

    public ReplyData GetFileData(InputData data)
    {
      var properties = new Hashtable
      {
        ["portName"] = Guid.NewGuid().ToString(),
        ["exclusiveAddressUse"] = false
      };
      var channel = new IpcChannel(properties, null, ServerSinkProvider);

      try
      {
        ChannelServices.RegisterChannel(channel, true);
      }
      catch
      {
        //the channel might be already registered, so ignore it
      }

      var uri = string.Format("ipc://{0}/{1}.rem", typeof(IContract).Name, typeof(RemoteObject).Name);
      var svc = Activator.GetObject(typeof(RemoteObject), uri) as IContract;

      var reply = svc.GetFileData(data);

      try
      {
        ChannelServices.UnregisterChannel(channel);
      }
      catch
      {
      }

      return reply;
    }
  }
}
