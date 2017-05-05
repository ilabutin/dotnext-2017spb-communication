using System;
using System.Security.Cryptography;

namespace DotNext
{

  public static class ServerLogic
  {
    public static ReplyData Convert(InputData data)
    {
      var reply = new ReplyData
      {
        Md5Hash = MD5.Create().ComputeHash(data.Content),
        Size = data.Content.Length
      };
      return reply;
    }
  }
}
