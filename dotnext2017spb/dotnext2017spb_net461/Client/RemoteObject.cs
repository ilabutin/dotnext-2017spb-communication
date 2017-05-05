using System;

namespace DotNext
{
  public class RemoteObject : MarshalByRefObject, IContract
  {
    private readonly IContract service;

    public RemoteObject()
    {
    }

    public RemoteObject(IContract service)
    {
      this.service = service;
    }

    public override object InitializeLifetimeService()
    {
      return null;
    }

    ReplyData IContract.GetFileData(InputData data)
    {
      if (this.service != null)
      {
        return this.service.GetFileData(data);
      }
      return null;
    }
  }
}
