using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNext
{
  public class WmCopyDataClient : IContract
  {
    private sealed class MessageHandler : NativeWindow
    {
      private readonly WmCopyDataClient client;

      public MessageHandler(WmCopyDataClient client)
      {
        Console.WriteLine("Creating reply window");
        this.client = client;
        CreateHandle(new CreateParams() { Caption = typeof(IContract).Name + "_reply" });
        Console.WriteLine("Reply window created");
      }

      protected override void WndProc(ref Message msg)
      {
        if (msg.Msg == WindowSender.WM_COPY_DATA)
        {
          Console.WriteLine("Reply received");
          var cds = (CopyDataStruct)Marshal.PtrToStructure(msg.LParam, typeof(CopyDataStruct));

          if (cds.cbData.ToInt32() > 0)
          {
            var bytes = new byte[cds.cbData.ToInt32()];
            Marshal.Copy(cds.lpData, bytes, 0, cds.cbData.ToInt32());
            client.replyBuf = bytes;
            Console.WriteLine("Reply decoded");
            client.replyReadyEvent.Set();
          }

          msg.Result = (IntPtr)1;
        }
        if (msg.Msg == WindowSender.WM_QUIT)
        {
          Console.WriteLine("Quit event for reply window received");
          Application.Exit();
        }

        base.WndProc(ref msg);
      }
    }

    private byte[] replyBuf;
    private readonly AutoResetEvent replyReadyEvent = new AutoResetEvent(false);
    private NativeWindow messageHandler;

    public ReplyData GetFileData(InputData data)
    {
      Task.Factory.StartNew(() =>
      {
        messageHandler = new MessageHandler(this);

        Application.Run();
      });

      var inputBuf = ByteArray.CreateFrom(data);
      WindowSender.SendMessage(typeof(IContract).Name + "_request", inputBuf);
      Console.WriteLine("Request sent");

      replyReadyEvent.WaitOne();
      WindowSender.SendQuitMessage(messageHandler.Handle);

      var replyData = replyBuf.ConvertTo<ReplyData>();
      return replyData;
    }
  }
}
