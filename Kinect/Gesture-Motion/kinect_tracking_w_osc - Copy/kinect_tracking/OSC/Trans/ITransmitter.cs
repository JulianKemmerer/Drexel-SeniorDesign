using System;
using Bespoke.Common.Osc;
using System.Net;

namespace kinect_tracking
{
    public interface ITransmitter
    {
        void Send(OscPacket packet);

        /// <summary>
        /// Stop the transmitter.
        /// </summary>
        void Stop();
    }
}
