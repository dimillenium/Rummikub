using Rummikub.Models;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Rummikub.Views
{
    public partial class ChoixAtout : Window
    {
        Tuile _tuile;

        public ChoixAtout(Tuile pTuile)
        {
            InitializeComponent();

            List<string> listeCouleurs = new List<string>();

            _tuile = pTuile;

            foreach (Tuile.Couleurs couleur in (Tuile.Couleurs[])Enum.GetValues(typeof(Tuile.Couleurs)))
                listeCouleurs.Add(couleur.ToString());

            comboCouleur.ItemsSource = listeCouleurs;
        }

        private void btnValiderChoixAtout_Click(object pSender, RoutedEventArgs pEvent)
        {
            if (comboCouleur.Text != "")
            {
                _tuile.Valeur = Convert.ToInt32(sliderValeur.Value);
                _tuile.Couleur = (Tuile.Couleurs)Enum.Parse(typeof(Tuile.Couleurs), comboCouleur.Text);

                DialogResult = true;

                return;
            }
        }
    }
}
