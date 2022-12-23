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
        private DataBase db; // Baza podataka elektrane
        private Client client; // Client za konekciju sa serverom

        public Powerplant(double snaga)
        {
            this.Snaga = snaga;
            this.Procenat_rada = 0;
            this.Trenutna_proizvodnja = 0;
            this.Db = new DataBase();
            this.Client = new Client();
        }

        public double Procenat_rada { get => procenat_rada; set => procenat_rada = value; }
        public double Snaga { get => snaga; set => snaga = value; }
        public double Trenutna_proizvodnja { get => trenutna_proizvodnja; set => trenutna_proizvodnja = value; }
        internal DataBase Db { get => db; set => db = value; }
        internal Client Client { get => client; set => client = value; }

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
