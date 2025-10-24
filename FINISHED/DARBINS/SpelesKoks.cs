using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARBINS
{
    internal class SpelesKoks
    {
        public List<SpelesKoks> Vecaki;
        public List<SpelesKoks> Berni;
        public string SpelesStavoklis;
        public int Limenis;
        public int CilvekaPunkti;
        public int DatoraPunkti;
        public int Svars = 0;
        public byte NavPagaiduIrSvars = 0;

        public SpelesKoks()
        {
            Vecaki = new List<SpelesKoks>();
            Berni = new List<SpelesKoks>();
        }
        public void PievienoVecaku(SpelesKoks vecaks)
        {
            Vecaki.Add(vecaks);
        }
        public void PievienoBernu(SpelesKoks berns)
        {
            Berni.Add(berns);
        }
    }
}
