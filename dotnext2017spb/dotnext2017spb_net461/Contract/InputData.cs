using System;
using System.Runtime.Serialization;

namespace DotNext
{
  [DataContract]
  [Serializable]
  public class InputData
  {
    [DataMember]
    public byte[] Content;
  }
}
