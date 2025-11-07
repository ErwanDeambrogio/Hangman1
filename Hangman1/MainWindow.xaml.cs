using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Hangman1
{
    public partial class MainWindow : Window
    {
        private string[] list_mots = {
    "CHAT", "CHIEN", "MAISON", "VOITURE", "TABLE", "CHAIR",
    "FLEUR", "ARBRE", "ROSE", "LUNE", "SOLEIL", "PAIN",
    "FROMAGE", "JAMBON", "POISSON", "VIANDE", "LEGUME", "FRUIT",
    "POMME", "BANANE", "ORANGE", "RAISIN", "FRAISE", "CERISE",
    "MONTAGNE", "RIVIERE", "LAC", "FORET", "DESERT", "PLAGE",
    "VENT", "PLUIE", "NEIGE", "TONNERRE", "OURS", "LOUP",
    "RENARD", "SOURIS", "LAPIN", "TIGRE", "LION", "SINGE",
    "VOITURE", "MOTO", "BUS", "TRAIN", "AVION", "BATEAU",
    "ECOLE", "COLLEGE", "LYCEE", "BIBLIOTHEQUE", "MUSEE",
    "THEATRE", "CINEMA", "PARC", "STADE", "HOPITAL",
    "DOCTEUR", "INFIRMIER", "PROFESSEUR", "CUISINIER",
    "POLICIER", "POMPIER", "AVOCAT", "JUGE", "ARTISTE",
    "MUSICIEN", "CHANTEUR", "DANSEUR", "PEINTRE", "POETE",
    "ACTEUR", "INFORMATIQUE", "LOGICIEL", "ALGORITHME",
    "JEU", "VIDEOGAME", "PUZZLE", "BALLON", "JOUET", "TIGE",
    "FEUILLE", "BRANCHE", "TRONC", "RACINE", "GRAINE",
    "CHOCOLAT", "GLACE", "BONBON", "CARAMEL", "CONFITURE",
    "TOMATE", "CAROTTE", "OIGNON", "AIL", "SEL", "POIVRE",
    "HUILE", "VINAIGRE", "EAU", "VIN", "BIERE", "THE",
    "CAFÉ", "PLAFOND", "MOQUETTE", "TAPIS", "RIDEAU",
    "CANAPE", "FAUTEUIL", "TABLEBASSE", "ARMOIRE", "COMMODO",
    "MATELAS", "OREILLER", "COUVERTURE", "DRAP", "LINGE",
    "SERVIETTE", "SAVON", "SHAMPOOING", "DOUCHE", "BAIN",
    "MIRROR", "TOILETTE", "BOL", "TASSE", "ASSIETTE",
    "CUILLERE", "FOURCHETTE", "COUTEAU", "VERRE", "LAMPE",
    "LUSTRE", "BOUGIE", "CHAUSSURE", "CHAUSSETTE", "PANTALON",
    "CHEMISE", "ROBE", "JUPE", "VESTE", "MANTEAU", "GANT",
    "BONNET", "ECHARPE", "CHAPEAU", "SAC", "PORTEFEUILLE",
    "CLE", "VOITURE", "VELO", "METRO", "TRAM", "STATION",
    "ROUTE", "RUE", "AVENUE", "PONT", "QUAI", "GARE",
    "AEROPORT", "PORT", "JETEE", "PHARE", "OCÉAN", "MER",
    "LAC", "RIVIERE", "FLEUVE", "PISCINE", "BASSIN", "FONTAINE",
    "STATUE", "MONUMENT", "BATIMENT", "CHATEAU", "ÉGLISE",
    "MOSQUEE", "TEMPLE", "SYNAGOGUE", "PARC", "SQUARE",
    "FOIRE", "MARCHE", "BOUTIQUE", "SUPERMARCHE", "ÉPICERIE",
    "BOULANGERIE", "PATISSERIE", "BOUCHERIE", "POISSONNERIE",
    "PHARMACIE", "LIBRAIRIE", "BANQUE", "CAISSE", "POSTE",
    "MAIRIE", "POLICE", "HOPITAL", "CLINIQUE", "LABORATOIRE",
    "UNIVERSITE", "COLLEGE", "LYCEE", "ECOLE", "MATERNELLE"
};



        private string motSecret;
        private char[] motAffiche;
        private int vies = 5;
        private int score = 0;
        private int streakt = 0;
        private int parties = 0;

        private DispatcherTimer timer;
        private int tempsRestant = 30; // en secondes

        private enum Difficulté { None, Easy, Medium, Hard }
        private Difficulté difficulteChoisie = Difficulté.None;
        private bool timerActive = false;

        public MainWindow()
        {
            InitializeComponent();

            // --- Au démarrage, tout invisible ---
            MettreToutInvisibleAvantChoixDifficulte();

            InitTimer();
            InitGame();
        }

        private void MettreToutInvisibleAvantChoixDifficulte()
        {
            // Désactiver lettres
            foreach (var ctrl in Grd_Keypad.Children)
            {
                if (ctrl is Button btn) btn.Visibility = Visibility.Hidden;
            }

            // Cœurs invisibles
            Vie1.Visibility = Visibility.Hidden;
            Vie2.Visibility = Visibility.Hidden;
            Vie3.Visibility = Visibility.Hidden;
            Vie4.Visibility = Visibility.Hidden;
            Vie5.Visibility = Visibility.Hidden;

            // Score, streak, parties invisibles
            Txt_Score.Visibility = Visibility.Hidden;
            Txt_Streak.Visibility = Visibility.Hidden;
            Txt_Parties.Visibility = Visibility.Hidden;

            // Timer invisible
            Txt_Timer.Visibility = Visibility.Hidden;
        }

        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tempsRestant--;

            if (tempsRestant <= 0)
            {
                tempsRestant = 0;
                UpdateTimerAffichage();
                GameOver(false);
                return;
            }

            UpdateTimerAffichage();
        }

        private void UpdateTimerAffichage()
        {
            if (!timerActive)
            {
                Txt_Timer.Visibility = Visibility.Hidden;
                return;
            }

            Txt_Timer.Visibility = Visibility.Visible;

            TimeSpan ts = TimeSpan.FromSeconds(tempsRestant);
            Txt_Timer.Text = ts.ToString(@"mm\:ss");

            Txt_Timer.Foreground = tempsRestant <= 10
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Yellow;
        }

        private void InitGame()
        {
            if (difficulteChoisie == Difficulté.None)
            {
                // Tout reste invisible
                MettreToutInvisibleAvantChoixDifficulte();
                return;
            }

            // Activer lettres et les rendre visibles
            foreach (var ctrl in Grd_Keypad.Children)
            {
                if (ctrl is Button btn)
                {
                    btn.IsEnabled = true;
                    btn.Background = new SolidColorBrush(Colors.Black);
                    btn.Visibility = Visibility.Visible;
                }
            }

            // Score/streak/parties visibles
            Txt_Score.Visibility = Visibility.Visible;
            Txt_Streak.Visibility = Visibility.Visible;
            Txt_Parties.Visibility = Visibility.Visible;

            Random rand = new Random();
            motSecret = list_mots[rand.Next(list_mots.Length)];
            motAffiche = motSecret.Select(c => '_').ToArray();
            Txt_MotCache.Text = string.Join(" ", motAffiche);

            // Vies selon difficulté
            switch (difficulteChoisie)
            {
                case Difficulté.Easy:
                case Difficulté.Medium:
                    vies = 5;
                    break;
                case Difficulté.Hard:
                    vies = 3;
                    break;
            }
            MiseAJourVies();

            // Image pendu
            Images_1.Source = new BitmapImage(new Uri("Images/1.png", UriKind.Relative));

            // Timer
            if (timerActive)
            {
                tempsRestant = 30;
                timer.Start();
            }
            else
            {
                timer.Stop();
                Txt_Timer.Visibility = Visibility.Hidden;
            }

            BtnRestart.Visibility = Visibility.Collapsed;
        }

        private void BTN_Letter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;

            MediaPlayer playMedia = new MediaPlayer();
            playMedia.Open(new Uri("Sons/click.mp3", UriKind.Relative));
            playMedia.Volume = 1;
            playMedia.Play();

            string lettre = btn.Content.ToString();
            bool bonneLettre = false;

            for (int i = 0; i < motSecret.Length; i++)
            {
                if (motSecret[i].ToString() == lettre)
                {
                    motAffiche[i] = lettre[0];
                    bonneLettre = true;
                }
            }

            Txt_MotCache.Text = string.Join(" ", motAffiche);
            btn.Background = new SolidColorBrush(Colors.Green);

            if (bonneLettre && timerActive) tempsRestant += 2;
            else if (!bonneLettre)
            {
                vies--;
                MiseAJourVies();
                if (timerActive) tempsRestant -= 5;
                btn.Background = new SolidColorBrush(Colors.Red);

                int indexImage = 6 - vies;
                if (indexImage < 1) indexImage = 1;
                if (indexImage > 6) indexImage = 6;
                Images_1.Source = new BitmapImage(new Uri($"Images/{indexImage}.png", UriKind.Relative));
            }

            UpdateTimerAffichage();

            if (vies <= 0) GameOver(false);
            else if (!motAffiche.Contains('_')) GameOver(true);
        }

        private void MiseAJourVies()
        {
            Vie1.Visibility = vies >= 1 ? Visibility.Visible : Visibility.Hidden;
            Vie2.Visibility = vies >= 2 ? Visibility.Visible : Visibility.Hidden;
            Vie3.Visibility = vies >= 3 ? Visibility.Visible : Visibility.Hidden;
            Vie4.Visibility = vies >= 4 ? Visibility.Visible : Visibility.Hidden;
            Vie5.Visibility = vies >= 5 ? Visibility.Visible : Visibility.Hidden;
        }

        private void GameOver(bool gagne)
        {
            timer.Stop();
            Txt_MotCache.Text = motSecret;

            if (gagne)
            {
                score += 10;
                streakt++;
            }
            else
            {
                streakt = 0;
            }
            parties++;
            UpdateScoreBoard();

            MessageBox.Show(gagne ? $"🎉 Bravo, le mot était : {motSecret}" : $"💀 Perdu ! Le mot était : {motSecret}");

            MediaPlayer playMedia = new MediaPlayer();
            playMedia.Open(new Uri(gagne ? "Sons/victoire.mp3" : "Sons/defaite.mp3", UriKind.Relative));
            playMedia.Volume = 1;
            playMedia.Play();

            foreach (var ctrl in Grd_Keypad.Children)
            {
                if (ctrl is Button btn)
                {
                    btn.Background = new SolidColorBrush(Colors.Black);
                    btn.IsEnabled = false;
                }
            }

            BtnRestart.Visibility = Visibility.Visible;
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

        private void UpdateScoreBoard()
        {
            Txt_Score.Text = score.ToString();
            Txt_Streak.Text = streakt.ToString();
            Txt_Parties.Text = parties.ToString();
        }

        // --- Gestion boutons difficulté ---
        private void Btn_Easy_Click(object sender, RoutedEventArgs e)
        {
            difficulteChoisie = Difficulté.Easy;
            timerActive = false;
            CacherBoutonsDifficulte();
            InitGame();
        }

        private void Btn_Medium_Click(object sender, RoutedEventArgs e)
        {
            difficulteChoisie = Difficulté.Medium;
            timerActive = true;
            CacherBoutonsDifficulte();
            InitGame();
        }

        private void Btn_Hard_Click(object sender, RoutedEventArgs e)
        {
            difficulteChoisie = Difficulté.Hard;
            timerActive = true;
            CacherBoutonsDifficulte();
            InitGame();
        }

        private void CacherBoutonsDifficulte()
        {
            Btn_Easy.Visibility = Visibility.Hidden;
            Btn_Medium.Visibility = Visibility.Hidden;
            Btn_Hard.Visibility = Visibility.Hidden;
        }
    }
}
