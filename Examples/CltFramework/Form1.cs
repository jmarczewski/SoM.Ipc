using SoM.Ipc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CltFramework
{
    public partial class Form1 : Form
    {
        IpcClient<WatsonTcpClientConnector> _ipc;
        CancellationTokenSource _cts = new CancellationTokenSource();
        
        SoM.Ipc.HubConnectionOptions _connectTo = new HubConnectionOptions()
        {
            Server = "127.0.0.1",
            Port = 50543,

        };

        public Form1()
        {
            InitializeComponent();

            _ipc = new IpcClient<WatsonTcpClientConnector>(null);
            _ipc.Connect(_connectTo);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer500ms_Tick(object sender, EventArgs e)
        {
            if (_ipc.IsConnected)
                labelConnectionStatus.Text = "Connected";
            else if ( _ipc.IsConnecting)
                labelConnectionStatus.Text = "Connecting";
            else
                labelConnectionStatus.Text = "Disconnected";

        }



        private async void buttonEvntA_Click(object sender, EventArgs e)
        {
            await _ipc.SendEventAsync(new MyIpc1.DemoIpcA() { Endpoint = "foo.evtA", PropA = "FooFeeFiFoFum" }, _cts.Token);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            _ipc.Connect(_connectTo);
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            _ipc.Disconnect();
        }

        private async void buttonReq_Click(object sender, EventArgs e)
        {
            MyIpc1.DemoIpcB result = (MyIpc1.DemoIpcB )await _ipc.SendRequestAsync(new MyIpc1.DemoIpcA() { Endpoint = "foo.pong", PropA = "Ping!" }, 
               TimeSpan.FromSeconds(4), _cts.Token);
            System.Diagnostics.Trace.WriteLine($"SoMIpc RX: {result}");
            labelPong.Text = $"[{DateTime.Now}] {result.PropB}";

        }
    }
}
