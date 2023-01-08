using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    public class Izvestaj
    {
        private List<Potrosac> potrosaci; // Lista svih potrosca
        public Dictionary<int,double> izvestaj; // Dicitonary id potrosca i ukupno vati koliko imaju


         
        public List<Potrosac> Potrosaci { get => potrosaci; set => potrosaci = value; }

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
        
        //Funkcija  uklanja potrosaca 
        public void Remove(int id)
        {
           //For petlja smanjuje id-je svih potrosca posle ovog koji treba biti uklonjen
           for(int i = id; i < potrosaci.Count; i++)
            {
                potrosaci[i].Id--;
                
            }
            potrosaci.RemoveAt(id - 1); // Ukljanja potrosca iz liste id-1 posto id-i krecu od 1 a nama lista krece od 0
            izvestaj.Clear(); //Brisemo ceo stari izvestaj
            Izracunaj_izvestaj(); //Pravimo novi izvestaj
        }

        // Funkcija updejtuje  listu izvestaj takodje  i updejtuje sve cene potrosnje korisnika takodje sinhornizuje listu potrosca sa listom u izvestaju po id
        public bool Izracunaj_izvestaj()
        {
            double tmp;
            for(int i = 0; i < Potrosaci.Count; i++)
            {
                if (izvestaj.ContainsKey(Potrosaci[i].Id)) //Proveravamo da li postoji potrosac sa tim idom u listi i updejtujemo vate
                {
                    tmp = Potrosaci[i].Cena_struje * Potrosaci[i].Vati/1000;
                    izvestaj[Potrosaci[i].Id] = tmp;
                }
                else if(!izvestaj.ContainsKey(Potrosaci[i].Id)) // Ako ne postoji potrosac ubacujemo ga kao par id i njegovih vati
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
