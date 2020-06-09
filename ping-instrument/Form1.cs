using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

// REFERENCE: https://docs.microsoft.com/en-us/dotnet/api/system.net.networkinformation.ping?view=netcore-3.1
namespace ping_instrument
{
    enum Device
    {
        //InstrumentA,
        //InstrumentB,
        InstrumentC
    }


    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); 
        }
        bool flag => checkBoxFlag.Checked;

        CancellationTokenSource _cts = null;
        SemaphoreSlim ssBusy = new SemaphoreSlim(1);

        private void ExecMulticastPing()
        {
            ssBusy.Wait();
            Task.Run(() =>
            {
                try
                {
                    _cts = new CancellationTokenSource();

                    do
                    {
                        List<Task<PingReply>> asyncPings = new List<Task<PingReply>>();
                        foreach (var device in (Device[])Enum.GetValues(typeof(Device)))
                        {
                            asyncPings.Add(Task.Run(() => SinglePingAsync(device, _cts.Token)));
                        }
                        Task.WaitAll(asyncPings.ToArray());

                        foreach (var device in (Device[])Enum.GetValues(typeof(Device)))
                        {
                        }

                        //Panel selectedPanel;
                        //foreach (var device in (Device[])Enum.GetValues(typeof(Device)))
                        //{
                        //    selectedPanel = (Panel)this.Controls.Find((device).ToString(), true)[0];
                        //    if (multicastPing.check(device.ToString()))
                        //        selectedPanel.BackgroundImage = Image.FromFile(Configs.imagesUrl + "enable\\" + selectedPanel.AccessibleName + ".png");
                        //    else
                        //        selectedPanel.BackgroundImage = Image.FromFile(Configs.imagesUrl + "disable\\" + selectedPanel.AccessibleName + ".png");
                        //}
                    } while (_cts.IsCancellationRequested);
                }
                finally
                {
                    ssBusy.Release();
                }
            });
        }

        // vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
        // Do this:
        const string URL_FOR_TEST = @"www.ivsoftware.com";
        // Not this (which will throw exception)
        // const string URL_FOR_TEST = @"http://www.ivsoftware.com";
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        private async Task<PingReply> SinglePingAsync(Device device, CancellationToken token)
        {
            if(token.IsCancellationRequested)
            {
                return null;
            }
            Ping pingSender = new Ping();
            pingSender.PingCompleted += PingSender_PingCompleted;
            PingOptions options = new PingOptions()
            {
                DontFragment = true
            };
            PingReply reply = null;
            await pingSender.SendPingAsync(URL_FOR_TEST);
            return reply;
        }

        private void PingSender_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                if (e.Reply.Status == IPStatus.Success)
                {
                    WriteLine("Address: " + e.Reply.Address.ToString());
                    WriteLine("RoundTrip time: " + e.Reply.RoundtripTime);
                    WriteLine("Time to live: " + e.Reply.Options.Ttl);
                    WriteLine("Don't fragment: " + e.Reply.Options.DontFragment);
                    WriteLine("Buffer size: " + e.Reply.Buffer.Length);
                }
                else
                {
                    WriteLine("REQUEST TIMEOUT");
                }
            });
        }

        private void WriteLine(string text)
        {
            richTextConsole.AppendText(text + Environment.NewLine);
        }

        private void checkBoxFlag_CheckedChanged(object sender, EventArgs e)
        {
            if(((CheckBox)sender).Checked)
            {
                _cts.Cancel();
            }
            else
            {
                ExecMulticastPing();
            }
        }
        class Image // Minimum Reproducable Excample Debug Only
        {
            public static System.Drawing.Image FromFile(string filename) 
            { 
                return null; // Debug only
            }
        }
        class Configs  // Minimum Reproducable Excample Debug Only
        {
            public static string imagesUrl { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\";
        }
    }
}
