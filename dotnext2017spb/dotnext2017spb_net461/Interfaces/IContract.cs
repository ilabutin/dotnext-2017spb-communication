using System.ServiceModel;
using DotNext;

namespace DotNext
{
  [ServiceContract]
  public interface IContract
  {
    [OperationContract]
    ReplyData GetFileData(InputData data);
  }
}
