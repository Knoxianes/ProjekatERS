using System;
using System.Collections.Generic;
using System.Text;

namespace Solar_panels_and__wind_generators
{
    class Wind_Generator
    {
        private double snaga_generatora;

        public Wind_Generator()
        {
        }

        public Wind_Generator(double snaga_generatora)
        {
            this.Snaga_generatora = snaga_generatora;
        }

        public double Snaga_generatora { get => snaga_generatora; set => snaga_generatora = value; }

        public override string ToString()
        {
            return "Snaga generatora je:" + snaga_generatora;
        }
    }
}
