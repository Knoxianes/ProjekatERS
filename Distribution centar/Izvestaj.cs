using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    class Izvestaj
    {
        private List<Potrosac> potrosaci;
        private Dictionary<int,double> izvestaj;

        public Izvestaj()
        {
            this.potrosaci = new List<Potrosac>();
            this.izvestaj = new Dictionary<int, double>();
        }

        // Funkcija dodaje novog potrosaca u listu potrosaca
        public void Add(int vati, double cena)
        {
            Potrosac tmp = new Potrosac(vati, cena);
            tmp.Id = potrosaci.Count+1;
            potrosaci.Add(tmp);
        }
        
        //Funkcija  uklanja potrosaca sa zadatim idom i updejtuje idove drugih potrosaca posle njega, vraca true ako je potrosac uspesno uklonjen
        public bool Remove(int id)
        {
            for(int i = id+1; i < potrosaci.Count;i ++)
            {
                potrosaci[i].Id--;
            }
            return potrosaci.Remove(potrosaci[id]);
            
        }

        // Funkcija updejtuje  listu izvestaj takodje  i updejtuje sve cene potrosnje korisnika takodje sinhornizuje listu potrosca sa listom u izvestaju po id
        public bool Izracunaj_izvestaj()
        {
            double tmp;
            for(int i = 0; i < potrosaci.Count; i++)
            {
                if (izvestaj.ContainsKey(potrosaci[i].Id))
                {
                    tmp = potrosaci[i].Cena_struje * potrosaci[i].Vati;
                    izvestaj[potrosaci[i].Id] = tmp;
                }
                else if(!izvestaj.ContainsKey(potrosaci[i].Id))
                {
                    tmp = potrosaci[i].Cena_struje * potrosaci[i].Vati;
                    izvestaj.Add(potrosaci[i].Id, tmp);
                }
                else
                {
                    Console.WriteLine("Doslo je do greske pri racunanju izvestaja kod racunanja cene!");
                    return false;
                }
            }
            foreach(int id in izvestaj.Keys)
            {
                for(int i = 0; i< potrosaci.Count; i++)
                {
                    if(id == potrosaci[i].Id)
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
