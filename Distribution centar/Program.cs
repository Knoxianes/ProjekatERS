using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Distribution_centar
{
    class Program
    {
        static void Main(string[] args)
        {
            var ds = new Distribution_center();
            while (true)
            {
                //Console.WriteLine(ds.Last_received_message_consumer);
               // Console.WriteLine(ds.Last_received_message_powerplant);
               // Console.WriteLine(ds.Last_received_message_solar_wind);
                //ds.Server_Send(ds.Stream_consumer, ds.Last_received_message_powerplant);
                //ds.Server_Send(ds.Stream_powerplant, ds.Last_received_message_solar_wind);
                //ds.Server_Send(ds.Stream_solar_wind, ds.Last_received_message_consumer);
                //Thread.Sleep(5000);
            }
            

        }
       
    }
}
