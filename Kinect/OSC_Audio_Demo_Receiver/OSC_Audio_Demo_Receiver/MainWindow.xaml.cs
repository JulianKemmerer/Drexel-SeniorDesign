using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using Bespoke.Common;
using Bespoke.Common.Osc;

namespace OSC_Audio_Demo_Receiver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer mp;

        public MainWindow()
        {
            InitializeComponent();

            OscServer oscServer;
            
            oscServer = new OscServer(TransportType.Udp, IPAddress.Loopback, Port);

            oscServer.FilterRegisteredMethods = false;
			oscServer.RegisterMethod(AliveMethod);
            oscServer.RegisterMethod(TestMethod);
            oscServer.BundleReceived += new EventHandler<OscBundleReceivedEventArgs>(oscServer_BundleReceived);
			oscServer.MessageReceived += new EventHandler<OscMessageReceivedEventArgs>(oscServer_MessageReceived);
            oscServer.ReceiveErrored += new EventHandler<Bespoke.Common.ExceptionEventArgs>(oscServer_ReceiveErrored);
            oscServer.ConsumeParsingExceptions = false;

            oscServer.Start();
            Console.WriteLine("Server started.");

            //Start playing audio
            string audioFileName = "/file/path";
            mp = new MediaPlayer();
            mp.MediaOpened += new EventHandler(mp_MediaOpened);
            bool isPaused = false;
            bool isPlaying = false;
        }

        void mp_MediaOpened(object sender, EventArgs e)
        {
            //Opening complete, play
            mp.Volume = 0.05;
            mp.Play();
            isPlaying = true;
        }

        private static void oscServer_BundleReceived(object sender, OscBundleReceivedEventArgs e)
        {
            sBundlesReceivedCount++;

            OscBundle bundle = e.Bundle;
            Console.WriteLine(string.Format("\nBundle Received [{0}:{1}]: Nested Bundles: {2} Nested Messages: {3}", bundle.SourceEndPoint.Address, bundle.TimeStamp, bundle.Bundles.Count, bundle.Messages.Count));
            Console.WriteLine("Total Bundles Received: {0}", sBundlesReceivedCount);
        }

		private static void oscServer_MessageReceived(object sender, OscMessageReceivedEventArgs e)
		{
            sMessagesReceivedCount++;

            OscMessage message = e.Message;

            Console.WriteLine(string.Format("\nMessage Received [{0}]: {1}", message.SourceEndPoint.Address, message.Address));
            Console.WriteLine(string.Format("Message contains {0} objects.", message.Data.Count));

            for (int i = 0; i < message.Data.Count; i++)
            {
                string dataString;

                if (message.Data[i] == null)
                {
                    dataString = "Nil";
                }
                else
                {
                    dataString = (message.Data[i] is byte[] ? BitConverter.ToString((byte[])message.Data[i]) : message.Data[i].ToString());
                }
                Console.WriteLine(string.Format("[{0}]: {1}", i, dataString));
            }

            Console.WriteLine("Total Messages Received: {0}", sMessagesReceivedCount);
		}

        private static void oscServer_ReceiveErrored(object sender, Bespoke.Common.ExceptionEventArgs e)
        {
            Console.WriteLine("Error during reception of packet: {0}", e.Exception.Message);
        }

		private static readonly int Port = 5103;
		private static readonly string AliveMethod = "/osctest/alive";
        private static readonly string TestMethod = "/osctest/test";

        private static int sBundlesReceivedCount;
        private static int sMessagesReceivedCount;
    }
}





namespace Receiver
{
    public enum DemoType
    {
        Udp,
        Tcp,
        Multicast
    }

	public class Program
	{
        
	}
}

