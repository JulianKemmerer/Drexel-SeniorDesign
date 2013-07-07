using System;
using System.Threading;
using System.Net;
using Bespoke.Common;
using Bespoke.Common.Osc;

namespace kinect_tracking
{
    public class UdpTransmitter : ITransmitter
    {
        public UdpTransmitter(IPEndPoint dest)
        {
            Destination = dest;
        }

        public void Send(OscPacket packet)
        {
            Assert.ParamIsNotNull(packet);

            mPacket = packet;

            mTransmitterThread = new Thread(RunWorker);
            mTransmitterThread.Start();
        }

        public void Stop()
        {
            mTransmitterThread.Join();
        }

        private void RunWorker()
        {
            try
            {
                mPacket.Send(Destination);            
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public IPEndPoint Destination;
        private Thread mTransmitterThread;
        private OscPacket mPacket;
    }
}
