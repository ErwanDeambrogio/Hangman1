using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Hangman1
{
    public partial class MainWindow : Window
    {
        private string[] list_mots = { "CHAT", "CHIEN", "MAISON", "ORDINATEUR", "VOITURE" };
        private string motMystere;
        private char[] motAffiche;
        private int vie = 5;

        private MediaPlayer playerClick;
        private MediaPlayer playerVictoire;
        private MediaPlayer playerDefaite;

        public MainWindow()
        {
            InitializeComponent();

            // Initialisation sons WAV
            playerClick = new MediaPlayer();
            playerClick.Open(new Uri("pack://application:,,,/Sons/click.wav"));

            playerVictoire = new MediaPlayer();
            playerVictoire.Open(new Uri("pack://application:,,,/Sons/victoire.wav"));

            playerDefaite = new MediaPlayer();
            playerDefaite.Open(new Uri("pack://application:,,,/Sons/defaite.wav"));

            InitialiserJeu();
        }

        private void InitialiserJeu()
        {
            Random rand = new Random();
            motMystere = list_mots[rand.Next(list_mots.Length)];
            motAffiche = new char[motMystere.Length];
            for (int i = 0; i < motMystere.Length; i++)
                motAffiche[i] = '_';

            Txt_MotCache.Text = string.Join(" ", motAffiche);
            vie = 5;
            UpdateImage();
            UpdateCoeurs();
            ReinitialiserBoutons();
        }

        private void BTN_Letter_Click(object sender, RoutedEventArgs e)
        {
            PlaySound(playerClick);

            Button btn = sender as Button;
            if (btn == null) return;

            char lettre = btn.Content.ToString()[0];
            btn.IsEnabled = false;

            bool trouve = false;
            for (int i = 0; i < motMystere.Length; i++)
            {
                if (motMystere[i] == lettre)
                {
                    motAffiche[i] = lettre;
                    trouve = true;
                }
            }

            Txt_MotCache.Text = string.Join(" ", motAffiche);

            if (!trouve)
            {
                btn.Background = Brushes.Red;
                vie--;
                UpdateImage();
                UpdateCoeurs();

                if (vie <= 0)
                {
                    PlaySound(playerDefaite);
                    MessageBox.Show("Perdu ! Le mot était : " + motMystere);
                    InitialiserJeu();
                }
            }
            else
            {
                btn.Background = Brushes.Green;
                if (!motAffiche.Contains('_'))
                {
                    PlaySound(playerVictoire);
                    MessageBox.Show("Gagné ! Le mot était : " + motMystere);
                    InitialiserJeu();
                }
            }
        }

        private void UpdateImage()
        {
            int indexImage = 6 - vie;
            if (indexImage < 1) indexImage = 1;
            if (indexImage > 6) indexImage = 6;

            Images_1.Source = new BitmapImage(new Uri($"Images/{indexImage}.png", UriKind.Relative));
        }

        private void UpdateCoeurs()
        {
            Image[] viesImages = { Vie1, Vie2, Vie3, Vie4, Vie5 };
            for (int i = 0; i < viesImages.Length; i++)
                viesImages[i].Visibility = vie > i ? Visibility.Visible : Visibility.Hidden;
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

        private void PlaySound(MediaPlayer player)
        {
            player.Stop();
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        // Bouton pour tester un son
        private void TestSon_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer testPlayer = new MediaPlayer();
            testPlayer.Open(new Uri("pack://application:,,,/Sons/test.wav"));
            testPlayer.Play();
        }
    }
}
