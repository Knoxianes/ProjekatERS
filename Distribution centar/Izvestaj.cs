using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    class Izvestaj
    {
        private List<Potrosac> potrosaci;
        private Dictionary<int,double> izvestaj;

        internal List<Potrosac> Potrosaci { get => potrosaci; set => potrosaci = value; }

        public Izvestaj()
        {
            this.Potrosaci = new List<Potrosac>();
            this.izvestaj = new Dictionary<int, double>();
        }

        // Funkcija dodaje novog potrosaca u listu potrosaca
        public void Add(int vati, double cena)
        {
            Potrosac tmp = new Potrosac(vati, cena);
            tmp.Id = Potrosaci.Count+1;
            Potrosaci.Add(tmp);
        }
        
        //Funkcija  uklanja potrosaca sa zadatim idom i updejtuje idove drugih potrosaca posle njega, vraca true ako je potrosac uspesno uklonjen
        public void Remove(int id)
        {
           for(int i = id; i < potrosaci.Count; i++)
            {
                potrosaci[i].Id--;
                
            }
            potrosaci.RemoveAt(id - 1);
            izvestaj.Clear();
            Izracunaj_izvestaj();
        }

        // Funkcija updejtuje  listu izvestaj takodje  i updejtuje sve cene potrosnje korisnika takodje sinhornizuje listu potrosca sa listom u izvestaju po id
        public bool Izracunaj_izvestaj()
        {
            double tmp;
            for(int i = 0; i < Potrosaci.Count; i++)
            {
                if (izvestaj.ContainsKey(Potrosaci[i].Id))
                {
                    tmp = Potrosaci[i].Cena_struje * Potrosaci[i].Vati/1000;
                    izvestaj[Potrosaci[i].Id] = tmp;
                }
                else if(!izvestaj.ContainsKey(Potrosaci[i].Id))
                {
                    tmp = Potrosaci[i].Cena_struje * Potrosaci[i].Vati/1000;
                    izvestaj.Add(Potrosaci[i].Id, tmp);
                }
                else
                {
                    Console.WriteLine("Doslo je do greske pri racunanju izvestaja kod racunanja cene!");
                    return false;
                }
            }
            return true;

        }
        public double Dobiti_Cenu_Struje(int id)
        {
            return izvestaj[id];
        }
  
        public override string ToString()
        {
            
            string tmp = "\n*************************************************";
            foreach (int id in izvestaj.Keys)
            {

                tmp += "\nPotrosac broj: " + id + " Cena potrosnje na sat vremena: " + izvestaj[id];

            }
            tmp += "\n*************************************************\n";

            return tmp;
        }

        

    }
}
