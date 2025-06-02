using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace NbaKartyaJatek
{
    public partial class MainWindow : Window
    {
        private List<Jatekos> jatekosok = new List<Jatekos>();
        private Queue<Jatekos> jatekosPakli = new Queue<Jatekos>();
        private Queue<Jatekos> gepPakli = new Queue<Jatekos>();
        private Random rnd = new Random();

        private int jatekosPont = 0;
        private int gepPont = 0;
        private int lapSzam = 20;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;
            if (!int.TryParse(LapSzamTextBox.Text, out lapSzam) || lapSzam <= 0 || lapSzam > 40 || lapSzam % 2 != 0)
            {
                ErrorText.Text = "Kérlek, adj meg egy páros számot 2 és 40 között!";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            MenuPanel.Visibility = Visibility.Collapsed;
            GamePanel.Visibility = Visibility.Visible;

            jatekosPont = 0;
            gepPont = 0;
            PontszamText.Text = $"Játékos {jatekosPont} - {gepPont} Gép";

            BetoltJatekosok();
            InditJatekot();
            MegjelenitAktualisLapok();
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            GamePanel.Visibility = Visibility.Collapsed;
            MenuPanel.Visibility = Visibility.Visible;

            // Töröljük a lapokat és pontokat visszaállítjuk 0ra
            jatekosPakli.Clear();
            gepPakli.Clear();
            jatekosPont = 0;
            gepPont = 0;
            PontszamText.Text = $"Játékos {jatekosPont} - {gepPont} Gép";

            JatekosKartyaLista.Items.Clear();
            GepKartyaLista.Items.Clear();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BetoltJatekosok()
        {
            jatekosok.Clear();

            string connStr = "server=localhost;port=3307;database=SportAdatbazis;uid=root;pwd=;";
            string query = "SELECT * FROM Jatekosok";

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var j = new Jatekos
                        {
                            Nev = reader["Nev"].ToString(),
                            Csapat = reader["Csapat"].ToString(),
                            Poszt = reader["Poszt"].ToString(),
                            Magassag = int.Parse(reader["Magassag"].ToString()),
                            Suly = int.Parse(reader["Suly"].ToString()),
                            Sebesseg = (int)Math.Round(float.Parse(reader["Sebesseg"].ToString())),
                            Dobas = (int)Math.Round(float.Parse(reader["Dobas"].ToString())),
                            Pontossag = (int)Math.Round(float.Parse(reader["Pontossag"].ToString()))
                        };
                        jatekosok.Add(j);
                    }
                }
            }
        }


        private void InditJatekot()
        {
            // Megkeverjük a teljes paklit
            var kevert = jatekosok.OrderBy(x => rnd.Next()).ToList();

            // Csak az első lapSzam darabot használjuk
            var valasztottLapok = kevert.Take(lapSzam).ToList();

            jatekosPakli.Clear();
            gepPakli.Clear();

            
            for (int i = 0; i < valasztottLapok.Count; i++)
            {
                if (i % 2 == 0)
                    jatekosPakli.Enqueue(valasztottLapok[i]);
                else
                    gepPakli.Enqueue(valasztottLapok[i]);
            }
        }

        private void KovetkezoKor_Click(object sender, RoutedEventArgs e)
        {
            if (jatekosPakli.Count == 0 || gepPakli.Count == 0)
            {
                MessageBox.Show("A játék véget ért!");
                return;
            }

            var jatekosLap = jatekosPakli.Dequeue();
            var gepLap = gepPakli.Dequeue();

            string[] tulajdonsagok = { "Sebesseg", "Dobas", "Pontossag" };
            string valasztott = tulajdonsagok[rnd.Next(tulajdonsagok.Length)];

            float jatekosErtek = GetErtek(jatekosLap, valasztott);
            float gepErtek = GetErtek(gepLap, valasztott);

            string eredmeny;

            if (jatekosErtek > gepErtek)
            {
                jatekosPont++;
                eredmeny = "Játékos nyert ezt a kört!";
                jatekosPakli.Enqueue(jatekosLap);
                jatekosPakli.Enqueue(gepLap);
            }
            else if (gepErtek > jatekosErtek)
            {
                gepPont++;
                eredmeny = "A gép nyert ezt a kört!";
                gepPakli.Enqueue(jatekosLap);
                gepPakli.Enqueue(gepLap);
            }
            else
            {
                eredmeny = "Döntetlen! A lapok elvesznek.";
            }

            PontszamText.Text = $"Játékos {jatekosPont} - {gepPont} Gép";

            MegjelenitAktualisLapok(jatekosLap, gepLap, valasztott, (int)jatekosErtek, (int)gepErtek, eredmeny);

            if (jatekosPakli.Count == 0 || gepPakli.Count == 0)
            {
                MessageBox.Show("A játék véget ért!");
            }
        }

        private void MegjelenitAktualisLapok(Jatekos jLap = null, Jatekos gLap = null, string tulajdonsag = "", int jErtek = 0, int gErtek = 0, string eredmeny = "")
        {
            JatekosKartyaLista.Items.Clear();
            GepKartyaLista.Items.Clear();

            if (jLap != null)
            {
                JatekosKartyaLista.Items.Add(CreateCard(jLap, tulajdonsag, jErtek));
            }
            if (gLap != null)
            {
                GepKartyaLista.Items.Add(CreateCard(gLap, tulajdonsag, gErtek));
            }
        }

        private Border CreateCard(Jatekos j, string valasztottTulajdonsag, int ertek)
        {
            var border = new Border
            {
                BorderBrush = System.Windows.Media.Brushes.White,
                BorderThickness = new Thickness(2),
                Margin = new Thickness(5),
                Padding = new Thickness(10),
                Width = 200,
                Background = System.Windows.Media.Brushes.DarkSlateGray,
                CornerRadius = new CornerRadius(8)
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock { Text = $"Név: {j.Nev}", Foreground = System.Windows.Media.Brushes.White, FontWeight = FontWeights.Bold, FontSize = 16 });
            stack.Children.Add(new TextBlock { Text = $"Csapat: {j.Csapat}", Foreground = System.Windows.Media.Brushes.White });
            stack.Children.Add(new TextBlock { Text = $"Pozíció: {j.Poszt}", Foreground = System.Windows.Media.Brushes.White });
            stack.Children.Add(new TextBlock { Text = $"Magasság: {j.Magassag} cm", Foreground = System.Windows.Media.Brushes.White });
            stack.Children.Add(new TextBlock { Text = $"Súly: {j.Suly} kg", Foreground = System.Windows.Media.Brushes.White });

            stack.Children.Add(new TextBlock
            {
                Text = $"Sebesség: {j.Sebesseg}" + (valasztottTulajdonsag == "Sebesseg" ? " ←" : ""),
                Foreground = System.Windows.Media.Brushes.White
            });
            stack.Children.Add(new TextBlock
            {
                Text = $"Dobás: {j.Dobas}" + (valasztottTulajdonsag == "Dobas" ? " ←" : ""),
                Foreground = System.Windows.Media.Brushes.White
            });
            stack.Children.Add(new TextBlock
            {
                Text = $"Pontosság: {j.Pontossag}" + (valasztottTulajdonsag == "Pontossag" ? " ←" : ""),
                Foreground = System.Windows.Media.Brushes.White
            });

            stack.Children.Add(new TextBlock
            {
                Text = $"Érték: {ertek}",
                Foreground = System.Windows.Media.Brushes.Yellow,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 5, 0, 0)
            });

            border.Child = stack;
            return border;
        }

        private float GetErtek(Jatekos j, string tulajdonsag)
        {
            switch (tulajdonsag)
            {
                case "Sebesseg": return j.Sebesseg;
                case "Dobas": return j.Dobas;
                case "Pontossag": return j.Pontossag;
                default: return 0;
            }
        }
    }

}
