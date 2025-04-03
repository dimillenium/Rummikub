using Rummikub.Classes;
using Rummikub.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rummikub.Views
{
    public partial class Jeu : Window
    {
        private List<Tuile> _tuilesSelectionneesPourDeplacement = new List<Tuile>();

        public Jeu()
        {
            InitializeComponent();

            Utils.Trace("Bienvenue à cette nouvelle partie de Rummikub " + Fonctions.ObtenirJoueurHumain().NomUtilisateur, Utils.CategorieTrace.INFO, txtTrace);

            if (Fonctions.ObtenirJoueurHumain().DernieresConnexions.Count == 1)
                Utils.Trace("Il s'agit de votre première connexion\n", Utils.CategorieTrace.INFO, txtTrace);
            else
                Utils.Trace("Votre dernière connexion remonte au " + Fonctions.ObtenirJoueurHumain().DernieresConnexions.ElementAt(1) + "\n", Utils.CategorieTrace.INFO, txtTrace);

            Utils.Trace("Affichage dynamique des joueurs", Utils.CategorieTrace.INFO, txtTrace);
            AfficherJoueurs();

            Utils.Trace("Préparation du jeu", Utils.CategorieTrace.INFO, txtTrace);
            MoteurJeu.PreparerJeu();

            Utils.Trace("Mélange des tuiles", Utils.CategorieTrace.INFO, txtTrace);
            Fonctions.MelangerJeu();

            Utils.Trace("Distribution des tuiles\n", Utils.CategorieTrace.INFO, txtTrace);
            MoteurJeu.DistribuerTuiles();

            AjouterTuilesSurChevalet();

            Utils.Trace("La partie peut commencer\n", Utils.CategorieTrace.INFO, txtTrace);
            Utils.Trace($"Le joueur qui commencera la partie sera : {MoteurJeu.JoueurCourant.NomUtilisateur}", Utils.CategorieTrace.INFO, txtTrace);

            //
            // Si HUMAIN n'est pas le joueur courant, on laisse commencer ORDI
            //
            if (MoteurJeu.JoueurCourant != MoteurJeu.FileJoueurs.Peek())
                JoueurSuivant();
        }

        private void AjouterTuileSurChevalet(StackPanel pStackPanelChevalet, Tuile pTuile)
        {
            MoteurJeu.GererDeplacementTuile(pStackPanelChevalet, pTuile, false);
            GererAffichage();
        }

        private void AjouterTuilesSurChevalet()
        {
            StackPanel stackPanelChevalet;

            stackPanelChevalet = FindName("stackPanelChevalet") as StackPanel;
            stackPanelChevalet.Uid = Guid.NewGuid().ToString();

            foreach (Tuile t in Fonctions.ObtenirJoueurHumain().TuilesEnMain)
                AjouterTuileSurChevalet(stackPanelChevalet, t);
        }

        private string AfficherListeTuiles()
        {
            return string.Join(", ", _tuilesSelectionneesPourDeplacement.Select(t => t.Valeur + " " + t.CouleurS()));
        }

        private void JoueurSuivant()
        {
            if (EstGagnant())
                return;

            //
            // Boucler de façon automatique sur les joueurs tant que ce n'est pas rendu à HUMAIN
            //
            do
            {
                if (!MoteurJeu.JoueurCourant.EstHumain)
                {
                    _tuilesSelectionneesPourDeplacement = MoteurJeu.TourOrdinateur();
                    if (EstGagnant())
                        return;

                    if (!MoteurJeu.JoueurCourant.EstDemarre)
                    {
                        //
                        // L'ORDINATEUR est en mesure de jouer son coup de départ
                        //
                        if (_tuilesSelectionneesPourDeplacement.Count >= MoteurJeu.NB_TUILE_PAR_SERIE_MINIMUM)
                        {
                            Utils.Trace($"{MoteurJeu.JoueurCourant.NomUtilisateur} a joué son coup de départ avec {AfficherListeTuiles()}\n", Utils.CategorieTrace.INFO, txtTrace);

                            MoteurJeu.JoueurCourant.EstDemarre = true;
                            StackPanel nouveauStackPanel = CreerStackPanel(MoteurJeu.FileJoueurs.ToList().FindIndex(j => j == MoteurJeu.JoueurCourant));
                            MoteurJeu.GererDeplacementTuiles(nouveauStackPanel, _tuilesSelectionneesPourDeplacement, false);
                        }
                        else
                            Utils.Trace($"{MoteurJeu.JoueurCourant.NomUtilisateur} a pioché une tuile\n", Utils.CategorieTrace.INFO, txtTrace);
                    }
                    else
                    {
                        if (_tuilesSelectionneesPourDeplacement.Count == 0)
                            Utils.Trace($"{MoteurJeu.JoueurCourant.NomUtilisateur} a pioché une tuile\n", Utils.CategorieTrace.INFO, txtTrace);
                        else
                            Utils.Trace($"{MoteurJeu.JoueurCourant.NomUtilisateur} joue la pièce {AfficherListeTuiles()}\n", Utils.CategorieTrace.INFO, txtTrace);
                    }

                }

                Fonctions.DeterminerJoueurSuivant();

                Utils.Trace("C'est au tour de " + MoteurJeu.JoueurCourant.NomUtilisateur + (MoteurJeu.JoueurCourant.EstDemarre ? "" : " (n'est pas démarré)") + " de jouer", Utils.CategorieTrace.INFO, txtTrace);
            } while (MoteurJeu.JoueurCourant != Fonctions.ObtenirJoueurHumain());


            //Rendu à humain?
            //Avant d'effacer, on retire aussi les tuiles de la pile
            if (MoteurJeu.PileDeplacements.Count > 0)
            {
                Tuile tuileARetirer;
                Tuple<StackPanel, Tuile> elementPile;
                string nomStackPanel;
                int nbElementPile = MoteurJeu.PileDeplacements.Count;
                for (int i = 0; i < nbElementPile; i++)
                {
                    elementPile = MoteurJeu.PileDeplacements.Pop();
                    nomStackPanel = elementPile.Item1.Name;
                    tuileARetirer = elementPile.Item2;
                    if (nomStackPanel == "stackPanelChevalet")
                    {
                        MoteurJeu.JoueurCourant.TuilesEnMain.Remove(tuileARetirer);
                    }
                }
                MoteurJeu.PileDeplacements.Clear();
            }

            _tuilesSelectionneesPourDeplacement.Clear();
            btnPiocherTuile.IsEnabled = true;
            GererAffichage();
        }

        private void GererAffichage()
        {
            //
            // Pour tous les StackPanels disponibles, on efface et on recrée les tuiles
            //
            foreach (StackPanel sp in MoteurJeu.SeriesSurJeu.Keys)
            {
                sp.Children.Clear();

                foreach (Tuile t in MoteurJeu.SeriesSurJeu[sp])
                    (TrouverUIElementParUID(gridPlancheJeu, Guid.Parse(sp.Uid)) as StackPanel).Children.Add(CreerTuile(t));
            }

            //
            // Affichage de la pile
            //
            txtPile.Text = null;
            AfficherPile(MoteurJeu.PileDeplacements);

            //
            // Affichage du nombre de tuiles des joueurs
            //
            for (int i = 0; i < MoteurJeu.NB_JOUEURS; i++)
                (FindName("tuileAffichage" + i) as ToggleButton).Content = MoteurJeu.FileJoueurs.ElementAt(i).TuilesEnMain.Count;

            _tuilesSelectionneesPourDeplacement.Clear();
        }

        private void AfficherPile(Stack<Tuple<StackPanel, Tuile>> pPile)
        {
            //
            // Si la pile est vide, on désactive le fait de pouvoir dépiler
            //
            if (pPile.Count == 0)
            {
                btnDepilerDeplacement.IsEnabled = false;

                return;
            }

            Tuple<StackPanel, Tuile> tuile = pPile.Peek();

            pPile.Pop();
            AfficherPile(pPile);

            txtPile.Text = (tuile.Item2 is Atout ? "(ATOUT) " : "") + tuile.Item2.Valeur + " " + tuile.Item2.CouleurS() + "\n" + txtPile.Text;

            btnDepilerDeplacement.IsEnabled = true;
            pPile.Push(tuile);
        }

        private bool EstGagnant()
        {
            string raisonGagnant = Fonctions.AnalyserSituationsGagnant();

            //
            // Est-ce qu'une situation (sac vide ou joueur) permet de définir un gagnant
            //
            if (!raisonGagnant.Equals(""))
            {
                Gagnant gagnant = new Gagnant(raisonGagnant);

                gagnant.Show();
                Close();

                return true;
            }

            return false;
        }
    }
}
