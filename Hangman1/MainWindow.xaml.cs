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


namespace Hangman1
{



    public partial class MainWindow : Window
    {

        // variable global
        Random rand = new Random();
        string[] list_mots = { "ordinateur", "souris", "clavier", "ecran", "telephone" };
        string selectedword;
        int vie = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void StartGame()
        {
            string mot = list_mots[rand.Next(list_mots.Length)];
            char[] motAffiche = new char[mot.Length];
            for (int i = 0; i < mot.Length; i++)
            {
                motAffiche[i] = '*';
            }

            vie = 5;

        }

        private void BTN_Letter_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;    
            if (btn != null)
            {
                string lettre = btn.Content.ToString(); 

            }
        }
    }
}

