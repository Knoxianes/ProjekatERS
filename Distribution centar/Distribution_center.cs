using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    class Distribution_center
    {
        private Izvestaj izvestaj;
        private Solar_wind solar_wind;
        private Powerplant powerplant;
        private double trenutna_proizvodnja;
        private double max_energije;
        private double potrebno_energije;

        public Distribution_center()
        {
            this.izvestaj = new Izvestaj();
            this.solar_wind = new Solar_wind();
            this.powerplant = new Powerplant();
            this.Trenutna_proizvodnja = 0;
            this.Max_energije = 10000;
            this.Potrebno_energije = 0;
        }

        public double Potrebno_energije { get => potrebno_energije; set => potrebno_energije = value; }
        public double Max_energije { get => max_energije; set => max_energije = value; }
        public double Trenutna_proizvodnja { get => trenutna_proizvodnja; set => trenutna_proizvodnja = value; }

        public override string ToString()
        {
            return "\n_______________\n" +
                "\n\tTrenutno stanje u celoj mrezi:\n" +
                "\tPotrebno energije: " + Potrebno_energije + "\n" +
                "\tTrenutna prozivodnja: " + trenutna_proizvodnja +
                solar_wind.ToString()+
                powerplant.ToString()+
                izvestaj.ToString()+
                "\n_______________\n";
        }
    }
}
