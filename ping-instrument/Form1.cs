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
                        if (_cts.IsCancellationRequested) break;

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
                        Task.Delay(1000).Wait();
                    } while (!_cts.IsCancellationRequested);
                }
                finally
                {
                    ssBusy.Release();
                }
                BeginInvoke((MethodInvoker)delegate
                {
                    WriteLine("CANCELLED");
                });
            });
        }

        // vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv
        // Do this:
        const string URL_FOR_TEST = @"www.ivsoftware.com";
        // Not this (which will throw exception)
        // const string URL_FOR_TEST = @"http://www.ivsoftware.com";
        // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        private PingReply SinglePingAsync(Device device, CancellationToken token)
        {
            if(token.IsCancellationRequested)
            {
                return null;
            }
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions()
            {
                DontFragment = true
            };
            PingReply reply = pingSender.Send(URL_FOR_TEST);
            BeginInvoke((MethodInvoker)delegate
            {
                if (reply.Status == IPStatus.Success)
                {
                    WriteLine("Address: " + reply.Address.ToString());
                    WriteLine("RoundTrip time: " + reply.RoundtripTime);
                    WriteLine("Time to live: " + reply.Options.Ttl);
                    WriteLine("Don't fragment: " + reply.Options.DontFragment);
                    WriteLine("Buffer size: " + reply.Buffer.Length);
                    WriteLine();
                }
                else
                {
                    WriteLine("REQUEST TIMEOUT");
                }
            });
            return reply;
        }

        private void WriteLine(string text="")
        {
            richTextConsole.AppendText(text + Environment.NewLine);
            richTextConsole.Select(richTextConsole.Text.Length, 0);
            richTextConsole.ScrollToCaret();
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
