using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USB_Term
{
    class Pomiar
    {
        private DateTime czas;
        private double t;
        private double h;

        public Pomiar(DateTime czas, double temperatura, double wilgotnosc)
        {
            this.czas = czas;
            t = temperatura;
            h = wilgotnosc;
        }

        public string odczyt()
        {
            return (czas + "\t\t|\t" + t + "*C\t\t|\t" + h + "%");
        }

        public double odczytTemp()
        {
            return (t);
        }

        public double odczytWilg()
        {
            return (h);
        }
    }
}
