using System;
using System.Threading;
using System.Net;
using Bespoke.Common;
using Bespoke.Common.Osc;

namespace kinect_tracking
{
    public class TcpTransmitter : ITransmitter
    {
        public TcpTransmitter(IPEndPoint dest)
        {
            Destination = dest;
        }

        public void Send(OscPacket packet)
        {
            Assert.ParamIsNotNull(packet);

            mOscClient = new OscClient(Destination);
            mOscClient.Connect();

            mPacket = packet;
            mPacket.Client = mOscClient;

            mTransmitterThread = new Thread(RunWorker);
            mTransmitterThread.Start();
        }

        public void Stop()
        {
            mTransmitterThread.Join();

            mOscClient.Close();
        }

        private void RunWorker()
        {
            try
            {
                mPacket.Send();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public IPEndPoint Destination;
        private Thread mTransmitterThread;
        private OscPacket mPacket;
        private OscClient mOscClient;        
    }
}
