using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    class Powerplant
    {
        private double snaga;
        private uint procenat_rada;

        public Powerplant()
        {
            this.Snaga = 0;
            this.Procenat_rada = 0;
        }

        public uint Procenat_rada { get => procenat_rada; set => procenat_rada = value; }
        public double Snaga { get => snaga; set => snaga = value; }

        public override string ToString()
        {
            return "\n\tTrenutna snaga elektrane je: " + Snaga + "\n" + "\tProcenat upotrebljenosti elektrane: " + procenat_rada + "\n";
        }
    }
}
