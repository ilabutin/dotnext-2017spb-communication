using System;
using System.IO;

namespace DotNext
{
  public static class StreamExtensions
  {
    private static byte[] ReceiveByteArray(Stream stream)
    {
      byte[] bufLen = new byte[4];
      if (stream.Read(bufLen, 0, 4) == 0)
      {
        return null;
      }
      int expectedLen = BitConverter.ToInt32(bufLen, 0);
      byte[] buf = new byte[expectedLen];
      int receivedLen = 0;
      while (receivedLen < expectedLen)
      {
        int read = stream.Read(buf, receivedLen, expectedLen - receivedLen);
        receivedLen += read;
      }
      return buf;
    }

    private static void SendByteArray(Stream stream, byte[] data)
    {
      byte[] inputBufLen = BitConverter.GetBytes(data.Length);
      stream.Write(inputBufLen, 0, inputBufLen.Length);
      stream.Write(data, 0, data.Length);
    }

    public static T Receive<T>(this Stream stream)
    {
      return ReceiveByteArray(stream).ConvertTo<T>();
    }

    public static void Send<T>(this Stream stream, T data)
    {
      SendByteArray(stream, ByteArray.CreateFrom(data));
    }
  }
}
