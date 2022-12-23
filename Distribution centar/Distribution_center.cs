using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Distribution_centar
{
    public class Distribution_center
    {
        private Izvestaj izvestaj; //Lista potrosaca
        private List<Solar_wind> solar_wind; //Lista svih solarnih panela i vetro generatora koji su konektovani
        private Powerplant powerplant; // Hidroelektrana 
        private double trenutna_proizvodnja; //Trenutno koliko prozivodi sistem ukupno struje
        private double potrebno_energije; // Koliko je energije potrebno potrosacima
        private NetworkStream stream_consumer; //Stream za slanje i primanje poruka od potrosaca
        private NetworkStream stream_powerplant; //Stream za slanje i primanje poruka od hidroeletrane
        private List<NetworkStream> stream_solar_wind; // Lista stremova za slanje i primanje poruka od svih clienta solarnih panela i vetro generatora
        private string last_received_message_consumer; // Zadnja poruka od potrosca
        private string last_received_message_powerplant;// Zadnja poruka od elektrane
        private List<string> last_received_message_solar_wind; // Lista zadnjih poruka od svih solarnih panela i vetro generatora
        private double cena; // Cena struje po kwh
      

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
            this.Cena = 50; // Ovde mozemo da izmenimo cenu struje ako zelimo
            _ = Server_Start(); // Pokretanje servera u konstruktoru
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
                var task_consumer = await Server_Recieve_Consumer(8000); // Pokretanje servera za primanje poruka potrosca
                var task_powerplant = await Server_Recieve_Powerplant(8001); // Pokretanje servera za primanje poruka elektrane
                var task_solar_wind = await Server_Recieve_Solar_wind(8002); // Pokretanje server za primanje poruka svih solarnih panela i vetro genetatora
                Task.WaitAll(task_consumer, task_powerplant,task_solar_wind); // Moramo sacekati da se svi konektuju da bi mogli da krenemo sa radom
                File.WriteAllText("Log\\log_distribution_center.txt", "Distribution center krece sa radom!\n");
                Server_Send(Stream_consumer, "1"); //Slanje koda da krene rad potrosca
                Server_Send(Stream_powerplant, "1");//Slanje koda da krene rad elektrane

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
                        try
                        {
                            byte[] buffer = new byte[1024];
                            stream_consumer.Read(buffer);                                            //Citanje poruke ako je primljena
                            last_received_message_consumer = Encoding.ASCII.GetString(buffer, 0, buffer.Length); //Dekodiranje poruke
                             WriteToFile("Server je primio poruku od consumera: " + last_received_message_consumer, "Log\\log_distribution_center.txt");
                            _ = Process_Consumer(); // Pokretanje funkcije za obradu poruke od potrosaca                                       
                        }
                        catch 
                        {

                        }
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
                            stream_powerplant.Read(buffer);                                            //Citanje poruke ako je primljena
                            last_received_message_powerplant = Encoding.ASCII.GetString(buffer, 0, buffer.Length); //Dekodiranje poruke
                             WriteToFile("Server je primio poruku od powerplanta: " + last_received_message_powerplant, "Log\\log_distribution_center.txt");
                            _ = Process_Powerplant();  //Pokretanje funkcije za bradu poruke od elektrane                                     
                           
                        }
                        catch
                        {

                        }
                        
                    }

                });
            });
            return await Task.FromResult(task);
        }

        // Funckija pokrece server na zadatom portu i pokrece funkcije za asihrono primanje konekcija
        private async Task<Task> Server_Recieve_Solar_wind(int port)
        {

            var task = Task.Factory.StartNew(() =>
            {
                var listener_client = new TcpListener(IPAddress.Any, port);    // Pravimo tcp socketa za slusanje na zadatom portu   
                listener_client.Start();      //Kretanje slusanja                                      
                Console.WriteLine("Krece slusanje za klijenta Solar_wind");
                _ =Solar_Wind_Accept(listener_client); // Pokrece funkciju za primanje novih konekcija              
            });
            return await Task.FromResult(task);
        }

        //Funkcija prihvatu novu konekciju na server i pokrece funkciju za stalno slusanje za prijem poruka na toj konekciji
        private async Task Solar_Wind_Accept(TcpListener listener_client)
        {
            _ =Task.Factory.StartNew(() => // Pokretanje novog Task za slusanje
            {
                while (true)
                {
                    
                    if (listener_client.Pending()) // Ako ima konekcija koje cekaju on ce ih obraditi
                    {
                        
                        var client = listener_client.AcceptTcpClient(); // Prihvatanje konekcije
                        Console.WriteLine("Client Solar_wind konektovan");
                        stream_solar_wind.Add(client.GetStream()); // Dodajemo stream u listu streamova
                        last_received_message_solar_wind.Add(""); // Dodavanje poruke u listu poruka
                        solar_wind.Add(new Solar_wind()); // Dodavanje nove instance klase Solar_wind u listu
                        _ = Solar_Wind_Recieve(stream_solar_wind.Count - 1); // Funckija koja pokrece primanje poruka od clienta a prodledjuje joj se indeks u listi stremova od strema koji treba da pokrene primanje poruka
                    }
                }
            });
        }

        //Funkcija pokrece Task za primanje poruka za zadati indeks strema u listi streamova
        private async Task Solar_Wind_Recieve(int index)
        {
            _ = Task.Factory.StartNew(() =>      //Pokrece novi Task                               
            {
                while (true)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        stream_solar_wind[index].Read(buffer); //Citanje poruke iz streama  i smestanje u buffer                                         
                        last_received_message_solar_wind[index] = Encoding.ASCII.GetString(buffer, 0, buffer.Length); //Dekodiranje poruke
                        WriteToFile("Server je primio poruku od windmill and solar panela: " + last_received_message_solar_wind[index], "Log\\log_distribution_center.txt");
                        _ = Process_Solar_Wind(index);    //Pokretanje funkcije za obradu poruke od clienta solarih panela i vetro generatora sa zadatim indeksom                                          
                    }
                    catch
                    {

                    }                                                                   
                }

            });
        }
        //Funkcija salje poruku na zadatim NetworkStream uz mogucnost dodavanja koda
        public async Task Server_Send(NetworkStream ns, string message, string code = "")
        {
            await Task.Factory.StartNew(() => {
                try
                {
                    if (code != "")
                    {
                        code += ";";
                    }
                    byte[] data = Encoding.ASCII.GetBytes(code + message); // Kodiranje poruke
                    ns.Write(data, 0, data.Length); // Slanje poruke kroz zadati stream
                    WriteToFile("Server je poslao poruku: " + code + message, "Log\\log_distribution_center.txt");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Doslo je do greske prilikom slanja poruke: " + e);

                }
            });
            
        }

        //Funkcija radi sve potrebne izmene, dodavanja i oduzimanja za potrosca
        private void Popunjavanje_Potrosaca(int broj)
        {
            var tmp = last_received_message_consumer.Split(";"); // Rasclanjivanje primljene poruke 

            //Svi sledeci ifovi proveravaju kod sa kojim je funkcija pozvana i na osnovu toga rade potrebnu radnju

            //1 Znaci prvo pokretanje i dodaje sadrzaj cele poruke u listu potrosca  i racuna potrebnu energiju 
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
            // 2 Znaci dodavanje novog potrosaca i update potrebne energije
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
             //3 Brisanje potrosca i update potrebne energije
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
            //4 Izmena potrosca sa zadatim idom i update potrebne energije
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
                            izvestaj.Izracunaj_izvestaj();                                                               // Update recnika izvestaja
                            Server_Send(Stream_consumer, "Uspesno su dodati svi potrosaci");                                                       // Slanje poruke klijentu da je gotova radnja
                            Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2"); // Slanje poruke elektrani razlika potreben energije i trenutno proizvedene 

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
                        izvestaj.Izracunaj_izvestaj();  // Update recnika izvestaja
                        Server_Send(Stream_consumer, "Uspesno je dodat potrosac"); // Slanje poruke potrosacu da je gotova radnja
                        Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2");  // Slanje poruke elektrani razlika potreben energije i trenutno proizvedene 



                }
                else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 3)
                    {
                        Popunjavanje_Potrosaca(3);
                        izvestaj.Izracunaj_izvestaj(); // Update recnika izvestaj
                        Server_Send(Stream_consumer, "Uspesno je izbrisan potrosac"); // Slanje poruke potrosacu da je gotova radnja
                        Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2"); // Slanje poruke elektrani razlika potreben energije i trenutno proizvedene 


                }
                else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 4)
                    {
                        Popunjavanje_Potrosaca(4);
                        izvestaj.Izracunaj_izvestaj(); // Update recnika izvestaja
                        Server_Send(Stream_consumer, "Uspesno je izmenjen potrosac"); // Slanje poruke potrosacu da je radnja gotova
                        Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2"); // Slanje poruke elektrani razlika potreben energije i trenutno proizvedene 


                }
                else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 5)
                    {
                        WriteToFile("Server salje izvestaj potrosacima! ","Log\\log_distribution_center.txt");
                        Server_Send(Stream_consumer, izvestaj.ToString()); // Slanje celog izvestaja po potrosacima clientu potrosac
                       
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
                            Server_Send(Stream_consumer, tmp); // Slanje izvestaja samo za jednog potrosaca
                           
                        }catch(Exception e)
                        {
                            Console.WriteLine("Doslo je do greske prilikom slanja izvestaja za jednog potrosca: " + e);
                            Environment.Exit(16);
                        }

                    }
                    else if (int.Parse(last_received_message_consumer.Split(";")[0]) == 7)
                    {
                        Console.WriteLine("\n*****PROGRAM SE GASI!!!*****");
                        Server_Send(Stream_powerplant, "STOP", "7"); // Slanje poruke sa kodom 7 da se elektrana ugasi
                        stream_powerplant.Close();
                        stream_consumer.Close();
                        for (int i = 0; i < stream_solar_wind.Count; i++)
                        {
                        
                            Server_Send(stream_solar_wind[i], "STOP", "7"); // Slanje poruke sa kodom 7 svim solarnim panelima i vetro generatorima da se ugase
                            stream_solar_wind[i].Close();
                        }
                        WriteToFile("SERVER SE GASI!", "Log\\log_distribution_center.txt");
                        Environment.Exit(0); // Server se gasi
                    }
            });
        }
        //Funckija u pozadini obradjuje zadnju primljenu poruku od elektrane
        private async Task  Process_Powerplant()
        {
            await Task.Factory.StartNew(() =>
            {
                    if (int.Parse(last_received_message_powerplant.Split(";")[0]) == 1) // Kod 1 oznaca 1 pokretanje clienta elektrane
                    {
                        powerplant.Snaga = double.Parse(last_received_message_powerplant.Split(";")[1]); // Postavljanje max snagu elektrane
                        
                    }
                    else if (int.Parse(last_received_message_powerplant.Split(";")[0]) == 2) //Kod 2 oznacava da je vec client pokrenut
                    {
                        powerplant.Procenat_rada = double.Parse(last_received_message_powerplant.Split(";")[1]); // Update procenat rada elektrane
                    }
            });
            
        }
        //Funckija u pozadini obradjuje zadnju primljenu poruku od solar panela i vetro generatora sa prosledjenim indeksom
        private async Task Process_Solar_Wind(int index)
        {
            await  Task.Factory.StartNew(() => 
            {
                var tmp = last_received_message_solar_wind[index].Split(";");
                if(int.Parse(tmp[0]) == 1) // 1 Oznacava prvo pokretanje clienta solar paneli i vetro generator
                {
                    solar_wind[index].Snaga = double.Parse(tmp[1]); 
                    trenutna_proizvodnja += double.Parse(tmp[1]); // Dodavanje snage na trenutnu porizvodnju
                    Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2"); // salje se poruka elektrani da bi prilagodila svoj rad
                }
                else if (int.Parse(tmp[0]) == 2) // 2 oznacava da nije prvo pokretanje
                {
                    trenutna_proizvodnja -= solar_wind[index].Snaga; // Brisanje stare snage od tih solarnih panela i vetro generatora iz trenutne proizovdnje
                    solar_wind[index].Snaga = double.Parse(tmp[1]);
                    trenutna_proizvodnja += double.Parse(tmp[1]); // Dodavanje nove snage na trenutnu proizvodnju
                    Server_Send(stream_powerplant, (potrebno_energije - trenutna_proizvodnja).ToString(), "2"); // salje se poruka elektrani da bi prilagodila svoj rad
                }
            }
            );
        }

        //Funckija pise u fajl iz zadatog patha
        public async Task WriteToFile(string msg, string path)
        {
          
            
                 await Task.Factory.StartNew(() => // Pravljenje posebnog Task za upisivanje poruke
                {

                    try
                    {
                        using StreamWriter w = new StreamWriter(path, append: true); // Otvaranje fajla
                        w.WriteLine(msg); // Pisanje poruke
                        w.Close(); //Zatvaranje fajla
                      
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
