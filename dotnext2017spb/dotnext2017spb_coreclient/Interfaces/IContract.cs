using System.ServiceModel;

namespace DotNext
{
  [ServiceContract]
  public interface IContract
  {
    [OperationContract]
    ReplyData GetFileData(InputData data);
  }
}
