using System;
using System.Runtime.InteropServices;

namespace DotNext
{
  [StructLayout(LayoutKind.Sequential)]
  public struct CopyDataStruct
  {
    public IntPtr dwData;
    public IntPtr cbData;
    public IntPtr lpData;
  }
}
