using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNext
{
  public class WmCopyDataServer : IServer
  {
    private NativeWindow messageHandler;

    private sealed class MessageHandler : NativeWindow
    {
      private readonly WmCopyDataServer server;

      public MessageHandler(WmCopyDataServer server)
      {
        Console.WriteLine("Creating request window");
        this.server = server;
        CreateHandle(new CreateParams() { Caption = typeof(IContract).Name + "_request" });
        Console.WriteLine("Request window created");
      }

      protected override void WndProc(ref Message msg)
      {
        if (msg.Msg == WindowSender.WM_COPY_DATA)
        {
          Console.WriteLine("Request received");
          var cds = (CopyDataStruct)Marshal.PtrToStructure(msg.LParam, typeof(CopyDataStruct));

          if (cds.cbData.ToInt32() > 0)
          {
            var bytes = new byte[cds.cbData.ToInt32()];
            Marshal.Copy(cds.lpData, bytes, 0, cds.cbData.ToInt32());
            Console.WriteLine("Request decoded");
            server.ProcessData(bytes);
          }

          msg.Result = (IntPtr)1;
        }
        if (msg.Msg == WindowSender.WM_QUIT)
        {
          Console.WriteLine("Request window closing");
          Application.Exit();
        }

        base.WndProc(ref msg);
      }
    }


    void IDisposable.Dispose()
    {
      WindowSender.SendQuitMessage(messageHandler.Handle);
    }

    public void Start()
    {
      Task.Factory.StartNew(() =>
      {
        messageHandler = new MessageHandler(this);

        Application.Run();
      });
    }

    private void ProcessData(byte[] inputBuf)
    {
      var inputData = inputBuf.ConvertTo<InputData>();
      var reply = ServerLogic.Convert(inputData);
      var replyBuf = ByteArray.CreateFrom(reply);
      WindowSender.SendMessage(typeof(IContract).Name + "_reply", replyBuf);
      Console.WriteLine("Reply sent");
    }
  }
}