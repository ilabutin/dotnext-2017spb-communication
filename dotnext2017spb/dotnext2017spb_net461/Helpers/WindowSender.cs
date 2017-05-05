using System;
using System.Runtime.InteropServices;

namespace DotNext
{
  public static class WindowSender
  {
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll")]
    private static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

    public const int WM_COPY_DATA = 0x004A;
    public const int WM_QUIT = 0x0012;

    public static void SendMessage(string windowName, byte[] buf)
    {
      IntPtr inputBufUnmanaged = Marshal.AllocHGlobal(buf.Length);
      Marshal.Copy(buf, 0, inputBufUnmanaged, buf.Length);

      var cds = new CopyDataStruct();
      cds.dwData = (IntPtr)Marshal.SizeOf(cds);
      cds.cbData = (IntPtr)buf.Length;
      cds.lpData = inputBufUnmanaged;

      var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));

      Marshal.StructureToPtr(cds, ptr, true);

      var target = FindWindow(null, windowName);

      var result = SendMessage(target, WM_COPY_DATA, IntPtr.Zero, ptr);

      Marshal.FreeHGlobal(cds.lpData);
      Marshal.FreeCoTaskMem(ptr);
    }

    public static void SendQuitMessage(IntPtr window)
    {
      SendMessage(window, WM_QUIT, IntPtr.Zero, IntPtr.Zero);
    }
  }
}
