using System;
using System.Collections.Generic;
using System.Text;

namespace Distribution_centar
{
    class Solar_Panel
    {
        private double snaga_panela;

        public Solar_Panel()
        {
        }

        public Solar_Panel(double snaga_panela)
        {
            this.Snaga_panela = snaga_panela;
        }

        public double Snaga_panela { get => snaga_panela; set => snaga_panela = value; }

        public override string ToString()
        {
            return "Snaga panela je: " + snaga_panela;
        }
    }
}
