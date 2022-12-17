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
        public bool Remove(int id)
        {
            for(int i = id+1; i < Potrosaci.Count;i ++)
            {
                Potrosaci[i].Id--;
            }
            return Potrosaci.Remove(Potrosaci[id]);
            
        }

        // Funkcija updejtuje  listu izvestaj takodje  i updejtuje sve cene potrosnje korisnika takodje sinhornizuje listu potrosca sa listom u izvestaju po id
        public bool Izracunaj_izvestaj()
        {
            double tmp;
            for(int i = 0; i < Potrosaci.Count; i++)
            {
                if (izvestaj.ContainsKey(Potrosaci[i].Id))
                {
                    tmp = Potrosaci[i].Cena_struje * Potrosaci[i].Vati;
                    izvestaj[Potrosaci[i].Id] = tmp;
                }
                else if(!izvestaj.ContainsKey(Potrosaci[i].Id))
                {
                    tmp = Potrosaci[i].Cena_struje * Potrosaci[i].Vati;
                    izvestaj.Add(Potrosaci[i].Id, tmp);
                }
                else
                {
                    Console.WriteLine("Doslo je do greske pri racunanju izvestaja kod racunanja cene!");
                    return false;
                }
            }
            foreach(int id in izvestaj.Keys)
            {
                for(int i = 0; i< Potrosaci.Count; i++)
                {
                    if(id == Potrosaci[i].Id)
                    {
                        break;
                    }
                    if (!izvestaj.Remove(id))
                    {
                        Console.WriteLine("Doslo je do greske pri racunanju izvestaja kod brisanja iz Dictionary");
                        return false;
                    }
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
            string tmp = "";
            foreach (int id in izvestaj.Keys)
            {
                tmp += "\n********************\n";
                tmp += "\tPotrosac broj: " + id + "\n" + "\tCena potrosnje na sat vremena: " + izvestaj[id];
                tmp += "\n********************\n";

            }

            return tmp;
        }

        

    }
}
