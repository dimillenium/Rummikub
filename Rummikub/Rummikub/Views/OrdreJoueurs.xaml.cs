using Rummikub.Classes;
using Rummikub.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;

namespace Rummikub.Views
{
    public partial class OrdreJoueurs : Window
    {
        public OrdreJoueurs()
        {
            InitializeComponent();

            AfficherJoueurs();
        }

        private void AfficherJoueurs()
        {
            for (int i = 0; i < MoteurJeu.NB_JOUEURS; i++)
            {
                (FindName("imgJoueur" + i) as Image).Source = new BitmapImage(new Uri("/Resources/Images/" + MoteurJeu.FileJoueurs.ElementAt(i).Personnage + ".gif", UriKind.Relative));
                (FindName("txtCompteJoueur" + i) as TextBlock).Text = MoteurJeu.FileJoueurs.ElementAt(i).NomUtilisateur;
            }
        }

        private void btnDeterminerOrdreJoueur_Click(object pSender, RoutedEventArgs pEvent)
        {
            if (MoteurJeu.JoueurCourant != null)
            {
                Jeu jeu = new Jeu();
                jeu.Show();

                Close();
                return;
            }

            int[] ordreJoueurs = Fonctions.DeterminerOrdreJoueurs();

            ToggleButton nouvelleTuile0 = new ToggleButton()
            {
                Content = ordreJoueurs[0]
            };

            Grid.SetColumn(nouvelleTuile0, 0);
            Grid.SetRow(nouvelleTuile0, 0);

            gridOrdreJoueurs.Children.Add(nouvelleTuile0);

            ToggleButton nouvelleTuile1 = new ToggleButton()
            {
                Content = ordreJoueurs[1]
            };

            Grid.SetColumn(nouvelleTuile1, 1);
            Grid.SetRow(nouvelleTuile1, 0);

            gridOrdreJoueurs.Children.Add(nouvelleTuile1);

            ToggleButton nouvelleTuile2 = new ToggleButton()
            {
                Content = ordreJoueurs[2]
            };

            Grid.SetColumn(nouvelleTuile2, 0);
            Grid.SetRow(nouvelleTuile2, 1);

            gridOrdreJoueurs.Children.Add(nouvelleTuile2);

            ToggleButton nouvelleTuile3 = new ToggleButton()
            {
                Content = ordreJoueurs[3]
            };

            Grid.SetColumn(nouvelleTuile3, 1);
            Grid.SetRow(nouvelleTuile3, 1);

            gridOrdreJoueurs.Children.Add(nouvelleTuile3);

            Utils.Trace($"Le premier joueur sera le joueur {MoteurJeu.JoueurCourant.NomUtilisateur}", Utils.CategorieTrace.INFO);

            btnDeterminerOrdreJoueur.Content = "Commencer la partie";
        }
    }
}
