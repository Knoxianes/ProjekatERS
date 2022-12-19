using System;
using System.IO;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            int tmp = int.Parse(client.Recieve());
            if(tmp != 1)
            {
                Console.WriteLine("Greska prilikom  startovanja servera!");
                Environment.Exit(1);
            }
            First_Start(client);
            while (true)
            {
                GUI(client);
            }

        }
        // Funkcija sluzi za prvo pokretanje clienta gde se unosi broj potrosaca i koliko koji potrosac trosi
        static void First_Start(Client client)
        {   
            int broj_potrosaca;
            do
            {
                Console.WriteLine("Koliko zelite potrosaca u sistemu? ");
                broj_potrosaca = int.Parse(Console.ReadLine());
                if (broj_potrosaca <= 0 || broj_potrosaca > 1000)
                {
                    Console.WriteLine("Uneli ste nemoguc broj potrosaca!");
                    continue;
                }
                else
                {
                    break;
                }
            } while (true);
            string msg = broj_potrosaca.ToString();
            for(int i = 1; i <= broj_potrosaca; i++)
            {
                Console.WriteLine("\nZa potrosca " + i + " koliko zelite televizora?");
                int broj_televizora = int.Parse(Console.ReadLine());
                Console.WriteLine("\nZa potrosca " + i + " koliko zelite sporeta?");
                int broj_sporeta = int.Parse(Console.ReadLine());
                Console.WriteLine("\nZa potrosca " + i + " koliko zelite frizidera?");
                int broj_frizidera = int.Parse(Console.ReadLine());
                msg = msg + ";" + Izracunaj_Potrosnju(broj_televizora, broj_sporeta, broj_frizidera);
            }
            
            client.Send(msg, "1");
            File.WriteAllText("Log\\log_consumer.txt", "Ka serveru je poslata poruka:" + msg + "\n");
        }
        

        //Funkcija izracunava potrosnju u vatima na osnovu broja tv sporeta i frizidera
        static double Izracunaj_Potrosnju(int broj_tv,int broj_sporeta, int broj_frizidera)
        {
            return (broj_tv * 200) + (broj_sporeta * 400) + (broj_frizidera * 300);
        }

        //GUI nakon prvog startovanja
        static void GUI( Client client)
        {
            Console.Clear();
            string msg = client.Recieve();
            client.WriteToFile("Consumer je primio poruku: " + msg, "Log\\log_consumer.txt");
            Console.WriteLine("*************** MENU ***************");
            Console.WriteLine("\t1.Dodati novog potrosaca");
            Console.WriteLine("\t2.Ukloniti potrosaca");
            Console.WriteLine("\t3.Izmeniti potrosaca");
            Console.WriteLine("\t4.Istampati izvestaje za sve potrosace");
            Console.WriteLine("\t5.Istampati izvestaj za jednog potrosaca");
            Console.WriteLine("\t6.Zatvoriti program");
            Console.Write("\n\n");
            Console.Write("Odgovor servera: ");
            Console.Write(msg +"\n");
            switch (Console.ReadKey().KeyChar){ // Svaki od kodova koji se salju su povezani sa kodovima na serverskoj strani
                case '1':
                    client.Send(Dodavanje_Potrosaca(),"2");
                    client.WriteToFile("Dodat je novi potrosac.", "Log\\log_consumer.txt");
                    break;
                case '2':
                    client.Send(Ukloniti_Potrosaca(), "3");
                    break;
                case '3':
                    client.Send(Izmeniti_Potrosaca(), "4");
                    break;
                case '4':
                    client.Send("", "5");
                    break;
                case '5':
                    client.Send(Istampati_Potrosaca(), "6");
                    break;
                case '6':
                    client.Send("", "7");
                    Console.WriteLine("\n * ****PROGRAM SE GASI!!! * ****");
                    Environment.Exit(0);
                    break;
                }
                
        
        }

        // GUI za dodavanje potrosaca
        static string Dodavanje_Potrosaca()
        {
            Console.Clear();
            Console.WriteLine("Koliko novi potrosac ima televizora?");
            var broj_televizora = int.Parse(Console.ReadLine());
            Console.WriteLine("Koliko novi potrosac ima sporeta?");
            var broj_sporeta = int.Parse(Console.ReadLine());
            Console.WriteLine("Koliko novi potrosac ima frizidera?");
            var broj_frizidera = int.Parse(Console.ReadLine());
            return Izracunaj_Potrosnju(broj_televizora, broj_sporeta, broj_frizidera).ToString();
        }

        //GUI za uklanjanje potrosaca
        static string Ukloniti_Potrosaca()
        {
            Console.Clear();
            Console.WriteLine("Potrosaca pod kojim ID-om zelite da uklonite?");
            return Console.ReadLine();
        }

        //GUI za izmenu potrosaca
        static string Izmeniti_Potrosaca()
        {
            Console.Clear();
            Console.WriteLine("Potrosaca pod kojim ID-om zelite da izmenite?");
            string id = Console.ReadLine();
            Console.WriteLine("Koliko potrosac "+ id + " ima televizora?");
            var broj_televizora = int.Parse(Console.ReadLine());
            Console.WriteLine("Koliko potrosac " + id + " ima sporeta?");
            var broj_sporeta = int.Parse(Console.ReadLine());
            Console.WriteLine("Koliko potrosac " + id + " ima frizidera?");
            var broj_frizidera = int.Parse(Console.ReadLine());
            return id + ";" + Izracunaj_Potrosnju(broj_televizora, broj_sporeta, broj_frizidera);

        }
        //GUI za stampanje potrosaca
        static string Istampati_Potrosaca()
        {
            Console.Clear();
            Console.WriteLine("Za potrosaca pod kojim ID-om zelite da dobijete izvestaj?");
            return Console.ReadLine();
        }
    }
}
