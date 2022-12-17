using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Distribution_centar
{
    class Distribution_center
    {
        private Izvestaj izvestaj;
        private Solar_wind solar_wind;
        private Powerplant powerplant;
        private double trenutna_proizvodnja;
        private double max_energije;
        private double potrebno_energije;
        private NetworkStream stream_consumer;
        private NetworkStream stream_powerplant;
        private NetworkStream stream_solar_wind;
        private string last_received_message_consumer;
        private string last_received_message_powerplant;
        private string last_received_message_solar_wind;

        public Distribution_center()
        {
            this.izvestaj = new Izvestaj();
            this.solar_wind = new Solar_wind();
            this.powerplant = new Powerplant();
            this.Trenutna_proizvodnja = 0;
            this.Max_energije = 10000;
            this.Potrebno_energije = 0;
            this.Stream_consumer = null;
            this.Stream_powerplant = null;
            this.Stream_solar_wind = null;
            this.Last_received_message_consumer = "";
            this.Last_received_message_powerplant = "";
            this.Last_received_message_solar_wind = "";
            Server_Start(); // Pokretanje servera u konstruktoru
        }

        public double Potrebno_energije { get => potrebno_energije; set => potrebno_energije = value; }
        public double Max_energije { get => max_energije; set => max_energije = value; }
        public double Trenutna_proizvodnja { get => trenutna_proizvodnja; set => trenutna_proizvodnja = value; }
        public NetworkStream Stream_consumer { get => stream_consumer; set => stream_consumer = value; }
        public NetworkStream Stream_powerplant { get => stream_powerplant; set => stream_powerplant = value; }
        public NetworkStream Stream_solar_wind { get => stream_solar_wind; set => stream_solar_wind = value; }
        public string Last_received_message_consumer { get => last_received_message_consumer; set => last_received_message_consumer = value; }
        public string Last_received_message_powerplant { get => last_received_message_powerplant; set => last_received_message_powerplant = value; }
        public string Last_received_message_solar_wind { get => last_received_message_solar_wind; set => last_received_message_solar_wind = value; }


        // Funkcija asihrono(ne blokirajuce) svaki od servera na zadatim portovima i salje svim clientima poruku da mogu da krenu sa radom
        private async Task Server_Start()
        {
            try
            {
                var task_consumer = await Server_Recieve_Consumer( 8000);
                var task_powerplant = await Server_Recieve_Powerplant(8001);
                var task_solar_wind = await Server_Recieve_Solar_wind(8002);
                Task.WaitAll(task_consumer, task_powerplant, task_solar_wind);
                Server_Send(Stream_consumer, "Start");
                Server_Send(Stream_powerplant, "Start");
                Server_Send(Stream_solar_wind, "Start");

            }
            catch
            {
                Console.WriteLine("Doslo je do greske prilikom paljenja servera!");
                Environment.Exit(1);
            }


        }

        // Funkcija pokrece kao poseban thread tcp server i kao poseban thread u pozadini pokrece petlju za prijem poruka
        private async Task<Task> Server_Recieve_Consumer(int port)
        {

            var task = Task.Factory.StartNew(() => // Taks.Factory.StartNew Pokrece novi thread
            {
                var listener_client = new TcpListener(IPAddress.Any, port);         // Pravljenje Tcp socketa na portu
                listener_client.Start();                                            // Pokretanje Tcp slusatelja 
                Console.WriteLine("Krece slusanje za klijenta Consumer");
                var client = listener_client.AcceptTcpClient();                     // Primanje tcp konekcije
                Console.WriteLine("Client Consumer konektovan");
                stream_consumer = client.GetStream();                               // Dobijanje strema za slanje poruka iz konekcije
                _ = Task.Factory.StartNew(() =>                                     // Pokretanje novog threda za while petlju za primanje poruka
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        stream_consumer.Read(buffer);                                            //Citanje poruke ako je primljena i jedna
                        last_received_message_consumer = Encoding.ASCII.GetString(buffer, 0, buffer.Length); //Dekodiranje poruke
                        Console.WriteLine(last_received_message_consumer);
                    }

                });
            });
            return await Task.FromResult(task);
        }

        // Funkcija pokrece kao poseban thread tcp server i kao poseban thread u pozadini pokrece petlju za prijem poruka
        private async Task<Task> Server_Recieve_Powerplant(int port)
        {

            var task = Task.Factory.StartNew(() => // Taks.Factory.StartNew Pokrece novi thread
            {
                var listener_client = new TcpListener(IPAddress.Any, port);         // Pravljenje Tcp socketa na portu
                listener_client.Start();                                            // Pokretanje Tcp slusatelja 
                Console.WriteLine("Krece slusanje za klijenta Powerplant");
                var client = listener_client.AcceptTcpClient();                     // Primanje tcp konekcije
                Console.WriteLine("Client Powerplant konektovan");
                stream_powerplant = client.GetStream();                               // Dobijanje strema za slanje poruka iz konekcije
                _ = Task.Factory.StartNew(() =>                                     // Pokretanje novog threda za while petlju za primanje poruka
                {
                    while (true)
                    {
                        try
                        {
                            byte[] buffer = new byte[1024];
                            stream_powerplant.Read(buffer);                                            //Citanje poruke ako je primljena i jedna
                            last_received_message_powerplant = Encoding.ASCII.GetString(buffer, 0, buffer.Length); //Dekodiranje poruke
                            Console.WriteLine(last_received_message_powerplant);
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Greska prilikom prijema poruke: " + e);
                            Environment.Exit(2);
                        }
                    }

                });
            });
            return await Task.FromResult(task);
        }

        // Funkcija pokrece kao poseban thread tcp server i kao poseban thread u pozadini pokrece petlju za prijem poruka
        private async Task<Task> Server_Recieve_Solar_wind(int port)
        {

            var task = Task.Factory.StartNew(() => // Taks.Factory.StartNew Pokrece novi thread
            {
                var listener_client = new TcpListener(IPAddress.Any, port);         // Pravljenje Tcp socketa na portu
                listener_client.Start();                                            // Pokretanje Tcp slusatelja 
                Console.WriteLine("Krece slusanje za klijenta Solar_wind");
                var client = listener_client.AcceptTcpClient();                     // Primanje tcp konekcije
                Console.WriteLine("Client Solar_wind konektovan");
                stream_solar_wind = client.GetStream();                               // Dobijanje strema za slanje poruka iz konekcije
                _ = Task.Factory.StartNew(() =>                                     // Pokretanje novog threda za while petlju za primanje poruka
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        stream_solar_wind.Read(buffer);                                            //Citanje poruke ako je primljena i jedna
                        last_received_message_solar_wind = Encoding.ASCII.GetString(buffer, 0, buffer.Length); //Dekodiranje poruke
                        Console.WriteLine(last_received_message_solar_wind);
                    }

                });
            });
            return await Task.FromResult(task);
        }
        //Funkcija salje poruku na zadatim NetworkStream uz mogucnost dodavanja koda
        public bool Server_Send(NetworkStream ns, string message, string code = "")
        {
            try
            {
                if (code != "")
                {
                    code += " ";
                }
                byte[] data = Encoding.ASCII.GetBytes(code + message);
                ns.Write(data, 0, data.Length);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom slanja poruke: " + e);
                return false;
            }
        }
        public override string ToString()
        {
            return "\n_______________\n" +
                "\n\tTrenutno stanje u celoj mrezi:\n" +
                "\tPotrebno energije: " + Potrebno_energije + "\n" +
                "\tTrenutna prozivodnja: " + trenutna_proizvodnja +
                solar_wind.ToString()+
                powerplant.ToString()+
                izvestaj.ToString()+
                "\n_______________\n";
        }
    }
}
