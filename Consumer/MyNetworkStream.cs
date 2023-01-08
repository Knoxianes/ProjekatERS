using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Consumer
{
    public class MyNetworkStream : IMyNetworkStream
    {
        public NetworkStream Stream { get; set; }

        
        public int Read(byte[] buffer)
        {
            return Stream.Read(buffer);
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            Stream.Write(buffer, offset, size);
        }
    }
}
