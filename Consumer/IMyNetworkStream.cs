using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Consumer
{
    public interface IMyNetworkStream
    {
        public NetworkStream Stream { get; set; }

        void Write(byte[] buffer, int offset, int size);

        int Read(byte[] buffer);
        
    }
}
