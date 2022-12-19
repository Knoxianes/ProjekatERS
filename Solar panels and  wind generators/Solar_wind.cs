﻿using System;
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
        private List<Solar_Panel> paneli;
        private List<Wind_Generator> generatori;
        private Timer timer;       

        public Solar_wind()
        {
            paneli = new List<Solar_Panel>();
            generatori = new List<Wind_Generator>();
            timer = new Timer();                  
            Korisnik_ui();
            Ponavljanje();            //promena vrednosti snage na odredjeno vreme

        }

        public void Korisnik_ui()
        {
            Console.Write("Unesite vrednost snage sunca: ");
            int sunce = int.Parse(Console.ReadLine());
            if (sunce < 0 && sunce > 100)
            {
                Console.WriteLine("Unesena vrednost nije u validnom opsegu.");
            }

            //slucajno generisanje vrednosti snage vetra 
            Random random = new Random();
            int vetar = random.Next(0, 101);
            Console.WriteLine("Vrednost snage vetra je: " + vetar);

            Add_panel(3, sunce);                                            //prvi argument je broj panela a drugi snaga
            Add_generator(2, vetar);                                        //prvi argument je broj generatora a drugi snaga
            Console.WriteLine("Ukupna snaga je: " + Ukupna_snaga());

            //DODAVANJE U BAZU 
            double p1 = paneli[0].Snaga_panela;
            double p2 = paneli[1].Snaga_panela;
            double p3 = paneli[2].Snaga_panela;

            double g1 = generatori[0].Snaga_generatora;
            double g2 = generatori[1].Snaga_generatora;
            
            string vreme = DateTime.Now.ToString("HH:mm:ss tt");

            string relativePath = @"..\..\Database1.mdf";
            string absolutePath = Path.GetFullPath(relativePath);

            string connectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True", absolutePath);

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand  command = new SqlCommand("insert into [Table] (Energija_Sunca,Energija_Vetra,Panel_1,Panel_2, Panel_3, Generator_1, Generator_2, Timestamp) values ('"+sunce+"', '"+vetar+"', '"+p1+"', '"+p2+"', '"+p3+"', '"+g1+"', '"+g2+"', '"+vreme+"')", connection);
            
            connection.Open();
            command.ExecuteNonQuery();
            
        }

        public void Ponavljanje()
        {
            //menjamo vrednosti na odredjeno vreme                                
            timer.Elapsed += new ElapsedEventHandler(Na_vreme);         //kazemo sta radi timer po isteku intervala
            timer.Interval = 3000;                                      //postavimo interval
            timer.Enabled = true;                                       //pokrenemo ga
        }

        //ista metoda kao korisnik_ui samo se ponavlja na odredjeno vreme
        public void Na_vreme(object source, ElapsedEventArgs evArgs)
        {
            timer.Stop();               //ako korisniku treba malo vise vremena da ukuca nego sto traje interval
            Korisnik_ui();
            timer.Start();
        }



        //generisanje vise instanci panela gde korisnik zadaje broj panela koji zeli (mozemo promeniti na fiksan broj)
        public bool Add_panel(int broj_panela, int snaga_sunca)                //korisnik zadaje i procenat snage
        {
            if (paneli.Count != 0)
                paneli.Clear();


            for(int i=0; i<broj_panela; i++)
            {
                Random rand = new Random();
                Solar_Panel panel = new Solar_Panel(rand.Next(320,350)*snaga_sunca/100);      //panel proizvede 340W na 100% snage 
                paneli.Add(panel);
            }

            if (paneli.Count == 0)
            {
                Console.WriteLine("Nije dodat ni jedan panel.\n");
                return false;
            }

            return true;
        }

        //generisanje vise instanci generatora gde korisnik zadaje broj generatora koji zeli (mozemo promeniti na fiksan broj)
        public bool Add_generator(int broj_generatora, int snaga_vetra)
        {
            if (generatori.Count != 0)
                generatori.Clear();

            for (int i = 0; i < broj_generatora; i++)
            {
                Random rand = new Random();
                Wind_Generator generator = new Wind_Generator(rand.Next(7900,8200) * snaga_vetra / 100);   //vetrenjaca proizvede 8200W na 100% snage      
                generatori.Add(generator);                                              
            }

            if (generatori.Count == 0)
            {
                Console.WriteLine("Nije dodat ni jedan generator.\n");
                return false;
            }

            return true;
        }

        //racunanje ukupne snage koju proizvode
        public double Ukupna_snaga()
        {
            double ukupno_paneli = 0;
            double ukupno_generatori = 0;

            foreach (var temp in paneli)
                ukupno_paneli += temp.Snaga_panela;

            foreach (var temp in generatori)
                ukupno_generatori += temp.Snaga_generatora;

            return ukupno_paneli + ukupno_generatori;
        }
    }
}
