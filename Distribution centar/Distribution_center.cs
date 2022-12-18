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
        private int flag_consumer;
        private int flag_powerplant;
        private int flag_solar_wind;
        private double cena;

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
            this.Flag_consumer = 0;
            this.Flag_powerplant = 0;
            this.Flag_solar_wind = 0;
            this.Cena = 50;
            _ = Server_Start(); // Pokretanje servera u konstruktoru
            _ = Process_Consumer(); // Pokretanje task za obradu consumera
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
        public int Flag_consumer { get => flag_consumer; set => flag_consumer = value; }
        public int Flag_powerplant { get => flag_powerplant; set => flag_powerplant = value; }
        public int Flag_solar_wind { get => flag_solar_wind; set => flag_solar_wind = value; }
        public double Cena { get => cena; set => cena = value; }


        // Funkcija asihrono(ne blokirajuce) svaki od servera na zadatim portovima i salje svim clientima poruku da mogu da krenu sa radom
        private async Task Server_Start()
        {
            try
            {
                var task_consumer = await Server_Recieve_Consumer( 8000);
                var task_powerplant = await Server_Recieve_Powerplant(8001);
                var task_solar_wind = await Server_Recieve_Solar_wind(8002);
                //Task.WaitAll(task_consumer, task_powerplant, task_solar_wind);
                Task.WaitAll(task_consumer);
                Server_Send(Stream_consumer, "1");
               // Server_Send(Stream_powerplant, "1");
                //Server_Send(Stream_solar_wind, "1");

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
                        Flag_consumer = 1;                                                  // Postavlja flag na 1 da bi program znao da je stigla poruka da moze da je obradi
                        //Console.WriteLine(last_received_message_consumer);
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
                            Flag_powerplant = 1;                                                          // Postavlja flag na 1 da bi program znao da je stigla poruka da moze da je obradi
                            // Console.WriteLine(last_received_message_powerplant);
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
                        Flag_solar_wind = 1;                                                   // Postavlja flag na 1 da bi program znao da je stigla poruka da moze da je obradi
                        // Console.WriteLine(last_received_message_solar_wind);
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
                    code += ";";
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

        //Funkcija radi sve potrebne izmene, dodavanja i oduzimanja za potrosca
        private void Popunjavanje_Potrosaca(int broj)
        {
            var tmp = last_received_message_consumer.Split(";");
            if (broj == 1)
            {
                try
                {
                    for (int i = 0; i < int.Parse(tmp[1]); i++)
                    {
                        var vati = int.Parse(tmp[i + 2]);
                        izvestaj.Add(vati, cena);
                        potrebno_energije += vati;
                        
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Doslo je do greske prilikom popunjavanja potrosaca: " + e);
                    Environment.Exit(11);
                }
            }else if(broj == 2)
            {
                try
                {
                    var vati = int.Parse(tmp[1]);
                    izvestaj.Add(vati, cena);
                    potrebno_energije += vati;

                }catch(Exception e)
                {
                    Console.WriteLine("Doslo je do greske prilikom dodavanja jednog potrosca: " + e);
                    Environment.Exit(12);
                }
            }else if(broj == 3)
            {
                try
                {
                    var id = int.Parse(tmp[1]);
                    izvestaj.Remove(id);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Doslo je do greske prilikom uklanjanja korisnika: " + e);
                    Environment.Exit(13);
                }

                }
            else if(broj == 4)
            {
                try
                {
                    var id = int.Parse(tmp[1]);
                    var vati = int.Parse(tmp[2]);
                    potrebno_energije = potrebno_energije - izvestaj.Potrosaci[id-1].Vati + vati;
                    izvestaj.Potrosaci[id-1].Vati = vati;

                }catch (Exception e)
                {
                    Console.WriteLine("Doslo je do greske pri menjanja vati kod korisnika: " + e);
                    Environment.Exit(14);
                }
                
            }
        }


        //Funkcija u pozadini obradjuje sve stvari vezane za potrosca, tacnije kada god potrosac nesto posalje serveru
        // na pocetku poruke se salje kod koji oznaca radnju koju server treba da odradi,
        // 1-prvo pokretanje,2 - dodavanje 1 potrosca, 3- brisanje potrosca, 4 - izmena nekog potrosca 5- vracanje izvestaja svih potrosca,
        // 6- vracanje izvestaja 1 potrosaca, 7- gasenje programa
        private async Task<Task> Process_Consumer()
        {
            var task = Task.Factory.StartNew(() => {
                while (true)
                {
                    if(Flag_consumer != 1)
                    {
                        continue;
                    }
                    if(int.Parse(last_received_message_consumer.Split(";")[0]) == 1)
                    {
                        try
                        {
                            Popunjavanje_Potrosaca(1);
                            izvestaj.Izracunaj_izvestaj();
                            Server_Send(Stream_consumer, "Done");
                            Flag_consumer = 0;
                            continue;
                        }catch (Exception e)
                        {
                            Console.WriteLine("Doslo je do greske prilikom obrade prve poruke potrosaca: " + e);
                            Environment.Exit(11);
                        } 
                    }
                    if (int.Parse(last_received_message_consumer.Split(";")[0]) == 2)
                    {
                        Popunjavanje_Potrosaca(2);
                        izvestaj.Izracunaj_izvestaj();
                        Server_Send(Stream_consumer, "Done");
                        Flag_consumer = 0;
                        continue;
                        
                    }
                    if (int.Parse(last_received_message_consumer.Split(";")[0]) == 3)
                    {
                        Popunjavanje_Potrosaca(3);
                        izvestaj.Izracunaj_izvestaj();
                        Server_Send(Stream_consumer, "Done");
                        Flag_consumer = 0;
                        continue;
                    }
                    if (int.Parse(last_received_message_consumer.Split(";")[0]) == 4)
                    {
                        Popunjavanje_Potrosaca(4);
                        izvestaj.Izracunaj_izvestaj();
                        Server_Send(Stream_consumer, "Done");
                        Flag_consumer = 0;
                        continue;
                    }
                    if (int.Parse(last_received_message_consumer.Split(";")[0]) == 5)
                    {

                        Server_Send(Stream_consumer, izvestaj.ToString());
                        Flag_consumer = 0;
                        continue;
                    }
                    if (int.Parse(last_received_message_consumer.Split(";")[0]) == 6)
                    {
                        try
                        {
                            var id = int.Parse(last_received_message_consumer.Split(";")[1]);
                            string tmp = "\n*************************************************";
                            tmp += "\nPotrosac broj: " + id + " Cena potrosnje na sat vremena: " + izvestaj.Dobiti_Cenu_Struje(id);
                            tmp += "\n*************************************************\n";
                            Server_Send(Stream_consumer, tmp);
                            Flag_consumer = 0;
                            continue;
                        }catch(Exception e)
                        {
                            Console.WriteLine("Doslo je do greske prilikom slanja izvestaja za jednog potrosca: " + e);
                            Environment.Exit(16);
                        }

                    }
                    if (int.Parse(last_received_message_consumer.Split(";")[0]) == 7)
                    {
                        Console.WriteLine("\n*****PROGRAM SE GASI!!!*****");
                        Server_Send(Stream_powerplant, "STOP", "7");
                        Server_Send(Stream_solar_wind, "STOP", "7");
                        Environment.Exit(0);

                    }



                }
            });
            return await Task.FromResult(task);
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
