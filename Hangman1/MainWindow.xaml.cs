using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace Hangman1
{
    public partial class MainWindow : Window
    {
        private string[] list_mots = {
        "CHAT", "CHIEN", "MAISON", "ORDINATEUR", "VOITURE", "PROGRAMME",
        "BATEAU", "AVION", "ELEPHANT", "GUITARE", "PIANO", "VIOLON",
        "LUNETTES", "TELEPHONE", "TABLE", "CHAISE", "MOTO", "ARBRES",
        "FLEUR", "SOLEIL", "NUAGE", "PLAGE", "MONTAGNE", "PISCINE",
        "FORET", "VILLE", "VILLAGE", "ECOLE", "BIBLIOTHEQUE", "MUSEE",
        "THEATRE", "CINEMA", "JARDIN", "SOURIS", "CLAVIER", "ECRAN",
        "IMPRIMANTE", "LAMPE", "TASSE", "ASSIETTE", "FOURCHETTE", "CUILLERE",
        "COUTEAU", "CANAPE", "VELO", "MOTOCROSS", "TENNIS", "BASKET",
        "FOOTBALL", "RUGBY", "HANDBALL", "NATATION", "COURSE", "CHOCOLAT",
        "BONBON", "GLACE", "PAIN", "FROMAGE", "FRUITS", "LEGUMES",
        "POISSON", "VIANDE", "SOUPE", "SALADE", "PIZZA", "HAMBURGER",
        "HOTDOG", "LOGICIEL", "APPLICATION", "JEUX", "INTERNET",
        "PROGRAMMATION", "LANGAGE", "ALGORITHME", "VARIABLE", "BOUCLE",
        "CONDITION", "FONCTION", "OBJET", "CLASSE", "INTERFACE", "METHODE",
        "PROJET", "MODULE", "EXCEPTION", "ERREUR", "TABLEAU", "CHAINE",
        "NOMBRE", "ENTIER", "DECIMAL", "BOOLEAN", "CONSTANTE", "BIBLIOTHEQUE",
        "DEVELOPPEUR", "INGENIEUR", "MEDECIN", "AVOCAT", "ARTISTE",
        "CHAUFFEUR", "CUISINIER", "ENSEIGNANT", "JOURNALISTE", "PHOTOGRAPHE",
        "PEINTRE", "SCULPTEUR", "ECRIVAIN", "MUSICIEN", "DANSEUR",
        "ACTEUR", "DIRECTEUR", "PILOTE", "CAPITAINE", "MARIN", "VOYAGE",
        "AVENTURE", "EXPLORATION", "DECISION", "STRATEGIE", "CONSEIL",
        "IDEE", "PROJET", "PLAN", "MISSION", "OBJETIF", "REUSSITE",
        "ECHEC", "BONHEUR", "TRISTESSE", "COLERE", "PEUR", "AMOUR",
        "AMITIE", "FAMILLE", "VOYAGE", "VACANCES", "PLAISIR", "JEU",
        "SANTE", "FORCE", "VITESSE", "AGILITE", "ENDURANCE", "EQUIPE",
        "COMPETITION", "TOURNOI", "MEDAILLE", "TROPHEE", "VICTOIRE",
        "DEFAITE", "CHALLENGE", "EXPERIENCE", "SAVOIR", "CONNAISSANCE",
        "ETUDE", "APPRENTISSAGE", "SCIENTIFIQUE", "TECHNOLOGIE", "INNOVATION",
        "ROBOT", "DRONE", "INTELLIGENCE", "ARTIFICIELLE", "VIRTUEL", "REALITE",
        "AUGMENTEE", "INTERNET", "SITEWEB", "APPLICATION", "SMARTPHONE",
        "TABLETTE", "ORDINATEUR", "SERVEUR", "BASEDEDONNEES", "RESEAU",
        "SECURITE", "CRYPTOGRAPHIE", "CODE", "PROGRAMMEUR", "DEVELOPPEMENT",
        "TEST", "DEBUG", "COMPILATION", "EXECUTION", "ERREUR", "CORRECTION",
        "MAISON", "APPARTEMENT", "CHAMBRE", "SALON", "CUISINE", "SALLEDEBAIN",
        "GARAGE", "JARDIN", "PISCINE", "BALCON", "TERRASSE", "TOIT",
        "MUR", "FENETRE", "PORTE", "SOL", "PLAFOND", "ESCALIER",
        "ELEVATEUR", "ASCENSEUR", "LUMIERE", "LAMPE", "TORCHE", "BOUGIE",
        "ORDINATEUR", "SOURIS", "CLAVIER", "ECRAN", "IMPRIMANTE", "CASQUE",
        "MICROPHONE", "ENCEINTE", "JEUVIDEO", "MANETTE", "TELECOMMANDE",
        "CABLE", "PRISE", "BATTERIE", "CHARGEUR"
        };

        private string motSecret;
        private char[] motAffiche;
        private int vies = 5;

        private DispatcherTimer timer;
        private int tempsRestant = 30; // en secondes

        public MainWindow()
        {
            InitializeComponent();
            InitGame();
            InitTimer();
        }

        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            UpdateTimerAffichage();
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
            TimeSpan ts = TimeSpan.FromSeconds(tempsRestant);
            Txt_Timer.Text = ts.ToString(@"mm\:ss");

            Txt_Timer.Foreground = tempsRestant <= 10
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Yellow;
        }

        private void InitGame()
        {
            Random rand = new Random();
            motSecret = list_mots[rand.Next(list_mots.Length)];

            motAffiche = motSecret.Select(c => '_').ToArray();
            Txt_MotCache.Text = string.Join(" ", motAffiche);

            vies = 5;
            MiseAJourVies();

            // Réinitialiser image pendu
            Images_1.Source = new BitmapImage(new Uri("Images/1.png", UriKind.Relative));

            // Réinitialiser timer
            tempsRestant = 30;
            UpdateTimerAffichage();

            // Réactiver toutes les lettres
            foreach (var ctrl in Grd_Keypad.Children)
            {
                if (ctrl is Button btn)
                {
                    btn.IsEnabled = true;
                    btn.Background = System.Windows.Media.Brushes.LightGray;
                }
            }

            // Masquer le bouton Restart
            BtnRestart.Visibility = Visibility.Collapsed;
        }

        private void BTN_Letter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;

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

            if (bonneLettre)
            {
                tempsRestant += 5;
            }
            else
            {
                vies--;
                tempsRestant -= 2;
                MiseAJourVies();

                // Mettre à jour image pendu
                int indexImage = 6 - vies;
                if (indexImage < 1) indexImage = 1;
                if (indexImage > 6) indexImage = 6;

                Images_1.Source = new BitmapImage(new Uri($"Images/{indexImage}.png", UriKind.Relative));
            }

            UpdateTimerAffichage();

            if (vies <= 0)
            {
                GameOver(false);
            }
            else if (!motAffiche.Contains('_'))
            {
                GameOver(true);
            }
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

            MessageBox.Show(gagne ? $"🎉 Bravo, le mot était : {motSecret}" : $"💀 Perdu ! Le mot était : {motSecret}");

            // Désactiver toutes les lettres
            foreach (var ctrl in Grd_Keypad.Children)
            {
                if (ctrl is Button btn) btn.IsEnabled = false;
            }

            // Afficher le bouton Restart
            BtnRestart.Visibility = Visibility.Visible;
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
            timer.Start();
        }
    }
}
