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
using System.Windows.Resources;
using System.Windows.Shapes;


namespace Hangman1
{



        public partial class MainWindow : Window
        {
            private string[] list_mots = { "chat", "chien", "maison", "ordinateur", "voiture" };
            private string motMystere;
            private char[] motAffiche;
            int vie = 5;

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
                btn.IsEnabled = false;
                vie--;
            }
                else
                {

                btn.Background = Brushes.Green;
                btn.IsEnabled = false;
                {

                
                }
                }
            }
        }
    }
