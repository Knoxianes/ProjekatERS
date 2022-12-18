using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    class Solar_wind
    {
        Dictionary<int, Solar_Panel> paneli;
        Dictionary<int, Wind_Generator> generatori;

        public Solar_wind()
        {
               paneli = new Dictionary<int, Solar_Panel>();
               generatori = new Dictionary<int, Wind_Generator>();
               korisnik();
        }

        public void korisnik()
        {
            //NATASA OVDE RADIS *************************************************
            add_panel(3, 70);   //prvi argument je broj panela a drugi snaga
            add_generator(2, 20);   //prvi argument je broj generatora a drugi snaga
            Console.WriteLine(ukupna_snaga());
        }

        //generisanje vise instanci panela gde korisnik zadaje broj panela koji zeli (mozemo promeniti na fiksan broj)
        public bool add_panel(int broj_panela, int snaga_sunca)                //korisnik zadaje i procenat snage
        {
            for(int i=0; i<broj_panela; i++)
            {                                             
                Solar_Panel panel = new Solar_Panel(350*snaga_sunca/100);      //panel proizvede 340W na 100% snage 
                paneli.Add(i, panel);
            }

            if (paneli.Count == 0)
            {
                Console.WriteLine("Nije dodat ni jedan panel.\n");
                return false;
            }

            return true;
        }

        //generisanje vise instanci generatora gde korisnik zadaje broj generatora koji zeli (mozemo promeniti na fiksan broj)
        public bool add_generator(int broj_generatora, int snaga_vetra)
        {
            for (int i = 0; i < broj_generatora; i++)
            {
                Wind_Generator generator = new Wind_Generator(8200 * snaga_vetra / 100);   //vetrenjaca proizvede 8200W na 100% snage      
                generatori.Add(i, generator);                                              
            }

            if (generatori.Count == 0)
            {
                Console.WriteLine("Nije dodat ni jedan generator.\n");
                return false;
            }

            return true;
        }

        //racunanje ukupne snage koju proizvode
        public double ukupna_snaga()
        {
            double ukupno_paneli = 0;
            double ukupno_generatori = 0;

            foreach (var temp in paneli)
                ukupno_paneli += temp.Value.Snaga_panela;

            foreach (var temp in generatori)
                ukupno_generatori += temp.Value.Snaga_generatora;

            return ukupno_paneli + ukupno_generatori;
        }
    }
}
