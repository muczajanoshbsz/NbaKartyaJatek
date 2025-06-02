using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NbaKartyaJatek
{
    public partial class JatekosKartya : UserControl
    {
        public JatekosKartya(Jatekos jatekos)
        {
            InitializeComponent();

            NevText.Text = jatekos.Nev;
            CsapatText.Text = jatekos.Csapat;
            PosztText.Text = $"Poszt: {jatekos.Poszt}";
            MagassagText.Text = $"Magasság: {jatekos.Magassag} cm";
            SulyText.Text = $"Súly: {jatekos.Suly} kg";
            SebessegText.Text = $"Sebesség: {jatekos.Sebesseg}";
            DobasText.Text = $"Dobás: {jatekos.Dobas}";
            PontossagText.Text = $"Pontosság: {jatekos.Pontossag}";
        }
    }
}
