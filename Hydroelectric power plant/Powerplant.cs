using System;
using System.Collections.Generic;
using System.Text;

namespace Hydroelectric_power_plant
{
    class Powerplant
    {
        private double snaga;
        private double procenat_rada;
        private double trenutna_proizvodnja;

        public Powerplant(double snaga)
        {
            this.Snaga = snaga;
            this.Procenat_rada = 0;
            this.Trenutna_proizvodnja = 0;
        }

        public double Procenat_rada { get => procenat_rada; set => procenat_rada = value; }
        public double Snaga { get => snaga; set => snaga = value; }
        public double Trenutna_proizvodnja { get => trenutna_proizvodnja; set => trenutna_proizvodnja = value; }

        public void  UpdateProcenat()
        {
            procenat_rada = (trenutna_proizvodnja / snaga) * 100;
        }
        public override string ToString()
        {
            return "\n\tTrenutna snaga elektrane je: " + Snaga  +"\n" +"\tProcenat upotrebljenosti elektrane: " + procenat_rada +"\n";
        }
    }
}
