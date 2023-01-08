using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    public class Potrosac
    {
        private int vati;
        private double cena_struje;
        private int id;

        public Potrosac(int vati, double cena_struje)
        {
            this.vati = vati;
            this.cena_struje = cena_struje;
            this.Id = 0;
        }

        public int Vati { get => vati; set => vati = value; }
        public double Cena_struje { get => cena_struje; set => cena_struje = value; }
        public int Id { get => id; set => id = value; }
    }
}
