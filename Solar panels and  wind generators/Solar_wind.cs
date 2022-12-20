using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Text;
using System.Timers;
using System.Data.SqlClient;
using System.IO;

namespace Distribution_centar
{
    class Solar_wind
    {
        private Solar_Panel panel;
        private Wind_Generator generator;
        private Timer timer;

        public Solar_Panel Panel { get => panel; set => panel = value; }
        public Wind_Generator Generator { get => generator; set => generator = value; }
        public Timer Timer { get => timer; set => timer = value; }

        public Solar_wind()
        {
            Panel = new Solar_Panel();
            Generator = new Wind_Generator();
            Timer = new Timer();                  
            Korisnik_ui();
            Ponavljanje();            //promena vrednosti snage na odredjeno vreme

        }

        public void Korisnik_ui()
        {
            int sunce;
            Console.WriteLine("Unesite vrednost snage sunca (broj od 0 do 100):");
            while (!(int.TryParse(Console.ReadLine(), out sunce)) || (sunce < 0 || sunce > 100))
            {
                Console.WriteLine("Nije uneta ispravna vrednost unesite opet: ");
            }            

            //slucajno generisanje vrednosti snage vetra 
            Random random = new Random();
            int vetar = random.Next(0, 101);
            Console.WriteLine("Vrednost snage vetra je: " + vetar);

            Panel = new Solar_Panel(350 * sunce / 100);
            Generator = new Wind_Generator(8200 * vetar / 100);

           
            Console.WriteLine("Ukupna snaga je: " + Ukupna_snaga(Panel.Snaga_panela, Generator.Snaga_generatora));

            //DODAVANJE U BAZU 
            double p = Panel.Snaga_panela;
            double g = Generator.Snaga_generatora;            
            string vreme = DateTime.Now.ToString("HH:mm:ss tt");

            //uzimamo relativnu putanju
            string relativePath = @"..\..\Database1.mdf";
            string absolutePath = Path.GetFullPath(relativePath);

            string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", absolutePath);

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand  command = new SqlCommand("insert into [Table] (Energija_Sunca,Energija_Vetra,Panel,Generator,Timestamp) values ('"+sunce+"', '"+vetar+"', '"+p+"', '"+g+"', '"+vreme+"')", connection);
            
            connection.Open();
            command.ExecuteNonQuery();           
        }

        public void Ponavljanje()
        {
            //menjamo vrednosti na odredjeno vreme                                
            Timer.Elapsed += new ElapsedEventHandler(Na_vreme);         //kazemo sta radi timer po isteku intervala
            Timer.Interval = 3000;                                      //postavimo interval
            Timer.Enabled = true;                                       //pokrenemo ga
        }

        //ista metoda kao korisnik_ui samo se ponavlja na odredjeno vreme
        public void Na_vreme(object source, ElapsedEventArgs evArgs)
        {
            Timer.Stop();               //ako korisniku treba malo vise vremena da ukuca nego sto traje interval
            Korisnik_ui();
            Timer.Start();
        }

        //racunanje ukupne snage koju proizvode
        public double Ukupna_snaga(double snaga_panela, double snaga_generatora)
        {
            return snaga_panela + snaga_generatora;
        }
    }
}
