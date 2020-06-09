using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
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
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            // Use the default Ttl value which is 128,
            // but change the fragmentation behavior.
            options.DontFragment = true;
        }
        bool flag = false;
        private void checkingDevice()
        {
            Task.Run(() =>
            {
                do
                {
                    Panel selectedPanel;
                    multicastPing.send();
                    foreach (var device in (Device[])Enum.GetValues(typeof(Device)))
                    {
                        selectedPanel = (Panel)this.Controls.Find((device).ToString(), true)[0];
                        if (multicastPing.check(device.ToString()))
                            selectedPanel.BackgroundImage = Image.FromFile(Configs.imagesUrl + "enable\\" + selectedPanel.AccessibleName + ".png");
                        else
                            selectedPanel.BackgroundImage = Image.FromFile(Configs.imagesUrl + "disable\\" + selectedPanel.AccessibleName + ".png");
                    }
                } while (!flag);
                // TODO
                // delete instrument object after using in this snippet code
            })
            .GetAwaiter()
            .OnCompleted(() =>
            {

            });
        }

        class multicastPing
        {

            public static bool send()
            {
                return true;
            }

            public static bool check(string name)
            {
                return true;
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
