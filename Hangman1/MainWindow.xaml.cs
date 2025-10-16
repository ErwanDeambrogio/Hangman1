using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hangman1
{
    public partial class MainWindow : Window
    {
        private string[] list_mots = { "chat", "chien", "maison", "ordinateur", "voiture" };
        private string motMystere;
        private char[] motAffiche;
        private int vie = 5;

        public MainWindow()
        {
            InitializeComponent();
            InitialiserJeu();
        }

        private void InitialiserJeu()
        {
            Random rand = new Random();
            motMystere = list_mots[rand.Next(list_mots.Length)];

            motAffiche = new char[motMystere.Length];
            for (int i = 0; i < motMystere.Length; i++)
            {
                motAffiche[i] = '_';
            }

            Txt_MotCache.Text = string.Join(" ", motAffiche);

            vie = 5;
            Lbl_Vies.Content = "Vies : " + vie;

            ReinitialiserBoutons();
        }

        private void BTN_Letter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            char lettre = btn.Content.ToString()[0];
            btn.IsEnabled = false;

            bool trouve = false;

            for (int i = 0; i < motMystere.Length; i++)
            {
                if (char.ToLower(motMystere[i]) == char.ToLower(lettre))
                {
                    motAffiche[i] = motMystere[i];
                    trouve = true;
                }
            }

            Txt_MotCache.Text = string.Join(" ", motAffiche);

            if (!trouve)
            {
                btn.Background = Brushes.Red;
                vie--;
                Lbl_Vies.Content = "Vies : " + vie;

                if (vie <= 0)
                {
                    MessageBox.Show("Perdu ! Le mot était : " + motMystere);
                    InitialiserJeu();
                }
            }
            else
            {
                btn.Background = Brushes.Green;

                if (!motAffiche.Contains('_'))
                {
                    MessageBox.Show("Gagné ! Le mot était : " + motMystere);
                    InitialiserJeu();
                }
            }
        }

        private void ReinitialiserBoutons()
        {
            foreach (var element in Grd_Keypad.Children)
            {
                if (element is Button btn)
                {
                    btn.IsEnabled = true;
                    btn.Background = Brushes.LightGray;
                }
            }
        }
    }
}
