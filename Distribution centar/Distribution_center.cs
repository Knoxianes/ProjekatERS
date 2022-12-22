using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Distribution_centar
{
    public class Distribution_center
    {
        private Izvestaj izvestaj;
        private List<Solar_wind> solar_wind;
        private Powerplant powerplant;
        private double trenutna_proizvodnja;
        private double potrebno_energije;
        private NetworkStream stream_consumer;
        private NetworkStream stream_powerplant;
        private List<NetworkStream> stream_solar_wind;
        private string last_received_message_consumer;
        private string last_received_message_powerplant;
        private List<string> last_received_message_solar_wind;
        private double cena;

        public Distribution_center()
        {
            this.izvestaj = new Izvestaj();
            this.solar_wind = new List<Solar_wind>();
            this.powerplant = new Powerplant();
            this.Trenutna_proizvodnja = 0;
            this.Potrebno_energije = 0;
            this.Stream_consumer = null;
            this.Stream_powerplant = null;
            this.stream_solar_wind = new List<NetworkStream>();
            this.Last_received_message_consumer = "";
            this.Last_received_message_powerplant = "";
            this.last_received_message_solar_wind = new List<string>();
            this.Cena = 50;
            _ = Server_Start(); // Pokretanje servera u konstruktoru
            //_ = Process_Consumer(); // Pokretanje task za obradu consumera
            //_ = Process_Powerplant(); // Pokretanje taska za obradu powerplanta
            //_ = Process_Solar_Wind();
        }

        public double Potrebno_energije { get => potrebno_energije; set => potrebno_energije = value; }
        public double Trenutna_proizvodnja { get => trenutna_proizvodnja; set => trenutna_proizvodnja = value; }
        public NetworkStream Stream_consumer { get => stream_consumer; set => stream_consumer = value; }
        public NetworkStream Stream_powerplant { get => stream_powerplant; set => stream_powerplant = value; }
        public string Last_received_message_consumer { get => last_received_message_consumer; set => last_received_message_consumer = value; }
        public string Last_received_message_powerplant { get => last_received_message_powerplant; set => last_received_message_powerplant = value; }
        public double Cena { get => cena; set => cena = value; }


        // Funkcija asihrono(ne blokirajuce) svaki od servera na zadatim portovima i salje svim clientima poruku da mogu da krenu sa radom
        private async Task Server_Start()
        {
            try
            {
                var task_consumer = await Server_Recieve_Consumer(8000);
                var task_powerplant = await Server_Recieve_Powerplant(8001);
                var task_solar_wind = await Server_Recieve_Solar_wind(8002);
                Task.WaitAll(task_consumer, task_powerplant,task_solar_wind);
                File.WriteAllText("Log\\log_distribution_center.txt", "Distribution center krece sa radom!\n");
                Server_Send(Stream_consumer, "1");
                Server_Send(Stream_powerplant, "1");

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
                        WriteToFile("Server je primio poruku od consumera: " + last_received_message_consumer, "Log\\log_distribution_center.txt");
                        _ = Process_Consumer();                                               
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
                            WriteToFile("Server je primio poruku od powerplanta: " + last_received_message_powerplant, "Log\\log_distribution_center.txt");
                            _ = Process_Powerplant();                                                        
                            // Console.WriteLine(last_received_message_powerplant);
                        }
                        catch (Exception e)
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

            var task = Task.Factory.StartNew(() =>
            {
                var listener_client = new TcpListener(IPAddress.Any, port);        
                listener_client.Start();                                            
                Console.WriteLine("Krece slusanje za klijenta Solar_wind");
                _ =Solar_Wind_Accept(listener_client);               
            });
            return await Task.FromResult(task);
        }
        private async Task Solar_Wind_Accept(TcpListener listener_client)
        {
            _ =Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    
                    if (listener_client.Pending())
                    {
                        int index = 0;
                        var client = listener_client.AcceptTcpClient();
                        Console.WriteLine("Client Solar_wind konektovan");
                        stream_solar_wind.Add(client.GetStream());
                        last_received_message_solar_wind.Add("");
                        solar_wind.Add(new Solar_wind());
                        index = stream_solar_wind.Count - 1;
                        _ = Solar_Wind_Recieve(index);
                    }
                }
            });
        }

        private async Task Solar_Wind_Recieve(int index)
        {
            _ = Task.Factory.StartNew(() =>                                     
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    stream_solar_wind[index].Read(buffer);                                            
                    last_received_message_solar_wind[index] = Encoding.ASCII.GetString(buffer, 0, buffer.Length); 
                    WriteToFile("Server je primio poruku od windmill and solar panela: " + last_received_message_solar_wind[index], "Log\\log_distribution_center.txt");
                    _ = Process_Solar_Wind(index);                                                
                                                                                           
                }

            });
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
                WriteToFile("Server je poslao poruku: " + code + message, "Log\\log_distribution_center.txt");
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
                WriteToFile("Server popunjava potrosace u izvestaj!", "Log\\log_distribution_center.txt");
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
                WriteToFile("Server dodaje novog korisnika.", "Log\\log_distribution_center.txt");
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
                WriteToFile("Server uklanja korisnika.", "Log\\log_distribution_center.txt");
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
                WriteToFile("Server menja korisnika:", "Log\\log_distribution_center.txt");
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
        private async Task Process_Consumer()
        {
            await Task.Factory.StartNew(() => {
               
                    if(int.Parse(last_received_message_consumer.Split(";")[0]) == 1)
                    {
                        try
                        {
                            Popunjavanje_Potrosaca(1);
                            izvestaj.Izracunaj_izvestaj();
                            Server_Send(Stream_consumer, "Done");
                            Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2");

                    }
                    catch (Exception e)
                        {
                            Console.WriteLine("Doslo je do greske prilikom obrade prve poruke potrosaca: " + e);
                            Environment.Exit(11);
                        } 
                    }
                    else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 2)
                    {
                        Popunjavanje_Potrosaca(2);
                        izvestaj.Izracunaj_izvestaj();
                        Server_Send(Stream_consumer, "Done");
                        Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2");


                    }
                    else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 3)
                    {
                        Popunjavanje_Potrosaca(3);
                        izvestaj.Izracunaj_izvestaj();
                        Server_Send(Stream_consumer, "Done");
                        Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2");

                   }
                    else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 4)
                    {
                        Popunjavanje_Potrosaca(4);
                        izvestaj.Izracunaj_izvestaj();
                        Server_Send(Stream_consumer, "Done");
                        Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2");

                    }
                    else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 5)
                    {
                        WriteToFile("Server salje izvestaj potrosacima! ","Log\\log_distribution_center.txt");
                        Server_Send(Stream_consumer, izvestaj.ToString());
                       
                    }
                    else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 6)
                    {
                        WriteToFile("Server salje izvestaj za jednog potrosaca!", "Log\\log_distribution_center.txt");
                        try
                        {
                            var id = int.Parse(last_received_message_consumer.Split(";")[1]);
                            string tmp = "\n*************************************************";
                            tmp += "\nPotrosac broj: " + id + " Cena potrosnje na sat vremena: " + izvestaj.Dobiti_Cenu_Struje(id);
                            tmp += "\n*************************************************\n";
                            Server_Send(Stream_consumer, tmp);
                           
                        }catch(Exception e)
                        {
                            Console.WriteLine("Doslo je do greske prilikom slanja izvestaja za jednog potrosca: " + e);
                            Environment.Exit(16);
                        }

                    }
                    else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 7)
                    {
                        Console.WriteLine("\n*****PROGRAM SE GASI!!!*****");
                        Server_Send(Stream_powerplant, "STOP", "7");
                        for (int i = 0; i < stream_solar_wind.Count; i++)
                        {
                            Server_Send(stream_solar_wind[i], "STOP", "7");
                        }
                        WriteToFile("SERVER SE GASI!", "Log\\log_distribution_center.txt");
                        Environment.Exit(0);
                    }
            });
        }
        private async Task  Process_Powerplant()
        {
            await Task.Factory.StartNew(() =>
            {
                    if (int.Parse(last_received_message_powerplant.Split(";")[0]) == 1)
                    {
                        powerplant.Snaga = double.Parse(last_received_message_powerplant.Split(";")[1]);
                        
                    }
                    else if (int.Parse(last_received_message_powerplant.Split(";")[0]) == 2)
                    {
                        powerplant.Procenat_rada = double.Parse(last_received_message_powerplant.Split(";")[1]);
                    }
            });
            
        }
        private async Task Process_Solar_Wind(int index)
        {
            await  Task.Factory.StartNew(() => 
            {
                var tmp = last_received_message_solar_wind[index].Split(";");
                if(int.Parse(tmp[0]) == 1)
                {
                    solar_wind[index].Snaga = double.Parse(tmp[1]);
                    trenutna_proizvodnja += double.Parse(tmp[1]);
                    Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2");
                }
                else if (int.Parse(tmp[0]) == 2)
                {
                    trenutna_proizvodnja -= solar_wind[index].Snaga;
                    solar_wind[index].Snaga = double.Parse(tmp[1]);
                    trenutna_proizvodnja += double.Parse(tmp[1]);
                    Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2");
                }
            }
            );
        }
        public async Task WriteToFile(string msg, string path)
        {
            await Task.Factory.StartNew(() =>
            {
                
                try
                {
                    using StreamWriter w = new StreamWriter(path, append: true);
                    w.WriteLine(msg);
                    w.Close();

                }
                catch (Exception e)
                {
                   
                }
            });
           
        }
        public override string ToString()
        {
            return "\n_______________\n" +
                "\n\tTrenutno stanje u celoj mrezi:\n" +
                "\tPotrebno energije: " + Potrebno_energije + "\n" +
                "\tTrenutna prozivodnja: " + trenutna_proizvodnja +
                solar_wind.ToString() +
                powerplant.ToString() +
                izvestaj.ToString() +
                "\n_______________\n";
        }

    }
}
