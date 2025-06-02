using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbaKartyaJatek
{
    public class Jatekos
    {
        public string Nev { get; set; }
        public string Csapat { get; set; }
        public string Poszt { get; set; }
        public int Magassag { get; set; }    
        public int Suly { get; set; }       
        public float Sebesseg { get; set; } 
        public float Dobas { get; set; }   
        public float Pontossag { get; set; }

        public override string ToString()
        {
            return $"{Nev} ({Csapat}) - Poszt: {Poszt}, Sebesség: {Sebesseg}, Dobás: {Dobas}, Pontosság: {Pontossag}";
        }
    }
}

