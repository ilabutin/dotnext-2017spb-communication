using System.Runtime.Serialization;

namespace DotNext
{
  [DataContract]
  public class InputData
  {
    [DataMember]
    public byte[] Content;
    [DataMember]
    public string Name;
  }
}
