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
        InstrumentA,
        InstrumentB,
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

                        // Sends out the pings in rapid succession to execute asynchronously in parallel
                        foreach (var device in (Device[])Enum.GetValues(typeof(Device)))
                        {
                            asyncPings.Add(Task.Run(() => SinglePingAsync(device, _cts.Token)));
                        }

                        // Waits for all the async pings to complete.
                        Task.WaitAll(asyncPings.ToArray());

                        // See if flag is already cancelled
                        if (_cts.IsCancellationRequested) break;

                        foreach (var device in (Device[])Enum.GetValues(typeof(Device)))
                        {
                            SetPanelImage(device, asyncPings[(int)device].Result);
                        }

                        // I believe that it's very important to throttle this to
                        // a reasonable number of repeats per second.
                        Task.Delay(1000).Wait();
                        BeginInvoke((MethodInvoker)delegate
                        {
                            WriteLine(); // Newline
                        });

                    } while (!_cts.IsCancellationRequested); // Check if it's cancelled now
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

        private void SetPanelImage(Device device, PingReply reply)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                WriteLine("Setting panel image for " + device.ToString() + " " + reply.Status.ToString() );
                Panel selectedPanel = (
                    from Control unk in Controls
                    where
                        (unk is Panel) &&
                        (unk.Name == device.ToString()) // ... or however you go about finding the panel...
                    select unk as Panel
                ).FirstOrDefault();

                if (selectedPanel != null)
                {
                    switch (reply.Status)
                    {
                        case IPStatus.Success:
                            // Set image for enabled
                            break;
                        case IPStatus.TimedOut:
                            // Set image as disabled
                            break;
                        default:
                            // Set image as disabled
                            break;
                    }
                }
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
    }
}
