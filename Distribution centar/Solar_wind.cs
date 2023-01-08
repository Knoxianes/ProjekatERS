using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    public class Solar_wind
    {
        private double snaga;

        public Solar_wind()
        {
            this.Snaga = 0;
        }

        public double Snaga { get => snaga; set => snaga = value; }

        public override string ToString()
        {
            return "\n\t Snaga vetrenjaca i solarnih panela je: " + snaga + "\n";
        }
    }
}