using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Text;
using System.Timers;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace Solar_panels_and__wind_generators
{
    public class Solar_wind
    {
        private Solar_Panel panel;
        private Wind_Generator generator;
        private Timer timer;
        private DataBase db;
        private int interval;
        private Client client;

        internal Solar_Panel Panel { get => panel; set => panel = value; }
        internal Wind_Generator Generator { get => generator; set => generator = value; }
        public Timer Timer { get => timer; set => timer = value; }
        internal DataBase Db { get => db; set => db = value; }
        public int Interval { get => interval; set => interval = value; }
        internal Client Client { get => client; set => client = value; }

        public Solar_wind()
        {
            this.Panel = new Solar_Panel();
            this.Generator = new Wind_Generator();
            this.Timer = new Timer();
            this.Db = new DataBase();
            this.Interval = int.Parse(ConfigurationManager.AppSettings["interval"]);
            this.Client = new Client();
            
        }
        // Funkcija koja poziva funkciju za genresianje vrednosti sunca i vetra nakon odredjenog vremena
        public void Ponavljanje()
        {
            //menjamo vrednosti na odredjeno vreme
            Timer timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(GenerateRandomValues); //kazemo sta radi timer po isteku intervala
            timer.Interval = Interval;                                      //postavimo interval
            timer.Enabled = true;                                       //pokrenemo ga
        }
        //Funkcija za generisanje vrednosti sunca  u vetra
        public void GenerateRandomValues(object source, ElapsedEventArgs evArgs)
        {
            Random random = new Random();
            double sunce = random.NextDouble() * 101;
            double vetar = random.NextDouble() * 101;
            panel.Snaga_panela = 350 * sunce / 100;
            generator.Snaga_generatora = 8200 * vetar / 100;
            Console.WriteLine("Snaga panela je: " + Math.Round(panel.Snaga_panela,2));
            Console.WriteLine("Snaga generatora je: " + Math.Round(generator.Snaga_generatora,2));
            client.Send((panel.Snaga_panela + generator.Snaga_generatora).ToString(), "2"); // Slanje snagu solarnih panela i vetro generatora serveru
            string vreme = DateTime.Now.ToString("HH:mm:ss tt");
            var command = "insert into Solar_Wind (Energija_Sunca,Energija_Vetra,Panel,Generator,Timestamp) values ('" + Math.Round(sunce,2) + "', '" + Math.Round(vetar,2) + "', '" + Math.Round(panel.Snaga_panela,2) + "', '" + Math.Round(generator.Snaga_generatora,2) + "', '" + vreme + "')";
            db.SendCommand(command); // Slanje komande u bazu podataka 
        }
    }
}
