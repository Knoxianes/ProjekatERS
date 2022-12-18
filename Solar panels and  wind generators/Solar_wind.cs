﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

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
                Solar_Panel panel = new Solar_Panel(350*snaga_sunca/100);      //panel proizvede 340W na 100% snage 
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
                Wind_Generator generator = new Wind_Generator(8200 * snaga_vetra / 100);   //vetrenjaca proizvede 8200W na 100% snage      
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