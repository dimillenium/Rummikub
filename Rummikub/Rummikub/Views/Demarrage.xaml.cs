using Rummikub.Classes;
using Rummikub.Models;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rummikub.Views
{
    public partial class Demarrage : Window
    {
        public Demarrage()
        {
            InitializeComponent();

            Utils.InitialisationTrace("trace.txt");
            Utils.Trace("Trace initialisée avec succès", Utils.CategorieTrace.INFO);

            Utils.Trace("Chargement du dictionnaire des joueurs", Utils.CategorieTrace.INFO);
            Acces.LireDictionnaires();

            Utils.Trace("Démarrage de la chanson groovy", Utils.CategorieTrace.INFO);
            Utils.JouerChanson();
        }

        private void btnValiderConnexion_Click(object pSender, RoutedEventArgs pEvent)
        {
            if (btnValiderConnexion.Content.Equals("Réinitialiser"))
            {
                ReinitialiserFormulaire();
                btnValiderConnexion.Content = "Valider connexion";

                return;
            }

            RadioButton personnageSelectionne = gridSelectionPersonnage.Children.OfType<RadioButton>().FirstOrDefault(r => r.IsChecked.HasValue && r.IsChecked.Value);

            if (personnageSelectionne == null)
                GererMessageErreur("Un personnage doit être sélectionné");
            else
            {
                lblMessage.Content = "Validation du joueur en cours";

                if (Acces.EstInformationsConnexionValides(txtNomUtilisateur.Text, passMotDePasse.Password))
                {
                    Fonctions.InitialiserJoueurs(Acces.ObtenirUtilisateur(txtNomUtilisateur.Text, personnageSelectionne.Name));

                    OrdreJoueurs ordreJoueurs = new OrdreJoueurs();
                    ordreJoueurs.Show();

                    Close();

                    return;
                }
                else
                    GererMessageErreur("Nom d'utilisateur ou mot de passe invalide");
            }

            btnValiderConnexion.Content = "Réinitialiser";
        }

        private void btnCreerJoueur_Click(object pSender, RoutedEventArgs pEvent)
        {
            lblMessage.Content = "Création du joueur en cours";

            if (!Acces.EstUtilisateurCree(txtNomUtilisateur.Text, passMotDePasse.Password))
                GererMessageErreur("Nom d'utilisateur déjà existant");
        }

        private void Connexion_Closing(object pSender, CancelEventArgs pEvent)
        {
            Acces.EcrireDictionnaires();
        }

        private void GererMessageErreur(string pMessageErreur)
        {
            lblMessage.Content = pMessageErreur;
            lblMessage.Foreground = Brushes.DarkRed;
            lblMessage.Background = Brushes.DarkOrange;
        }

        private void ReinitialiserFormulaire()
        {
            lblMessage.Content = "";
            lblMessage.Foreground = Brushes.Black;
            lblMessage.Background = Brushes.White;
        }
    }
}
