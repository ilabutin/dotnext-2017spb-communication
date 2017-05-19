using System.Runtime.Serialization;

namespace DotNext
{
  [DataContract]
  public class ReplyData
  {
    [DataMember]
    public long Size;
  }
}
