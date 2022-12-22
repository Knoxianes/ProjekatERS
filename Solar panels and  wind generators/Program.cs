
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Text;
using System.Timers;
using System.Data.SqlClient;
using System.IO;


namespace Solar_panels_and__wind_generators
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            Solar_wind solar_wind = new Solar_wind();
            Korisnik_ui(solar_wind.Panel, solar_wind.Generator, solar_wind.Db,client);
            solar_wind.Ponavljanje();
            while (true) { }

        }
       static public void Korisnik_ui(Solar_Panel s, Wind_Generator g, DataBase db,Client client)
        {
            double sunce, vetar;
            Console.WriteLine("Unesite procenat snage sunca (broj od 0 do 100):");
            while (!(double.TryParse(Console.ReadLine(), out sunce)) || (sunce < 0 || sunce > 100))
            {
                Console.WriteLine("Nije uneta ispravna vrednost unesite opet: ");
            }
            Console.WriteLine("Unesite vrednost snage vetra (broj od 0 do 100):");
            while (!(double.TryParse(Console.ReadLine(), out vetar)) || (vetar < 0 || vetar > 100))
            {
                Console.WriteLine("Nije uneta ispravna vrednost unesite opet: ");
            }
            s.Snaga_panela = 350 * sunce / 100;
            g.Snaga_generatora = 8200 * vetar / 100;
            Console.WriteLine("Snaga panela je: " + Math.Round(s.Snaga_panela,2));
            Console.WriteLine("Snaga generatora je: " + Math.Round(g.Snaga_generatora,2));
            string vreme = DateTime.Now.ToString("HH:mm:ss tt");
            var command = "insert into Solar_Wind (Energija_Sunca,Energija_Vetra,Panel,Generator,Timestamp) values ('" + sunce + "', '" + vetar + "', '" + s.Snaga_panela + "', '" + g.Snaga_generatora + "', '" + vreme + "')";
            db.SendCommand(command);
        }
        
    }
}
