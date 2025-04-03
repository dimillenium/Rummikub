using Rummikub.Models;
using Rummikub.Views;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace Rummikub.Classes
{
    public static partial class Fonctions
    {

        //
        // TODO On doit initialiser la file des joueurs.
        // L'objet Joueur qui correspond à l'utilisateur est passé en paramètre; cet objet doit être inséré et toujours resté en première position (début) de la file. Par la
        // suite, on doit "enfiler" à la suite 3 nouveaux joueurs (l'ordinateur) avec qui sera joué la partie (voir la classe Joueur, qui possède deux constructeurs).
        //
        // Chaque Joueur doit posséder un personnage différent qui le représente visuellement. La liste des personnages est la suivante:
        // - green, mustard, peacock, plum, scarlet, white.
        // La casse doit être respectée, car le visuel du jeu est basé sur ces noms.
        //
        // Le joueur humain passé en paramètre possède déjà son personnage; pour les 3 autres joueurs, un personnage doit être choisi aléatoirement à partir des personnages
        // restant.
        //

        /// <summary>
        /// Fonction qui initialise les joueurs pour débuter le jeu.
        /// </summary>
        /// <param name="pJoueurHumain">Un joueur humain qui va être initialisé</param>
        public static void InitialiserJoueurs(Joueur pJoueurHumain)
        {
            List<string> listNoms = new List<string>() {"green", "mustard", "peacock", "plum", "scarlet", "white" };

            MoteurJeu.FileJoueurs.Enqueue(pJoueurHumain);

            pJoueurHumain.TuilesEnMain = new List<Tuile>();

            pJoueurHumain.EstHumain = true;

            listNoms.Remove(pJoueurHumain.Personnage);

            Random random = new Random();

            for (int i = 0; i < MoteurJeu.NB_JOUEURS - 1; i++)
            {
                string nomOrdinateur = listNoms[random.Next(listNoms.Count)];
                Joueur ordinateur = new Joueur($"Ordinateur{i + 1}", nomOrdinateur);

                ordinateur.EstHumain = false;

                MoteurJeu.FileJoueurs.Enqueue(ordinateur);
                listNoms.Remove(nomOrdinateur);
            }
          
        }


    

        //
        // TODO Mélanger aléatoirement les tuiles d'une liste.
        // À partir des tuiles se trouvant dans MoteurJeu.Sac, on doit toutes les mélanger de façon aléatoire.
        //
        // À la fin de la fonction, MoteurJeu.Sac doit se retrouver avec les tuiles mélangées.
        //

        /// <summary>
        /// Fonction qui mélange le sac de tuiles pour débuter le jeu.
        /// </summary>
        public static void MelangerJeu()
        {
            Random random = new Random();
            List<Tuile> sac = MoteurJeu.Sac;

            for (int i = sac.Count - 1; i > 0; i--)
            {

                int indexAleatoire = random.Next(i + 1);


                Tuile temp = sac[i];
                sac[i] = sac[indexAleatoire];
                sac[indexAleatoire] = temp;
            }
        }


        //
        // TODO Piler la tuile jouée.
        // On doit piler la tuile jouée sur la pile (voir propriétés de MoteurJeu).
        //

        /// <summary>
        /// Fonction qui sert à piler un déplacement d'une tuile.
        /// </summary>
        /// <param name="pTuile">Une tuile qui va être pilée</param>
        public static void PilerDeplacement(Tuple<StackPanel, Tuile> pTuile)
        {
            MoteurJeu.PileDeplacements.Push(pTuile);
        }


        //
        // TODO On doit trier (ou grouper) une liste de tuiles passées en paramètre par couleur.
        // À partir de cette liste, toutes les tuiles de la même couleur sont regroupées ensembles. L'ordre des couleurs et l'ordre de la valeur des tuiles
        // n'importe pas, tout comme l'atout.
        //
        // La fonction retourne la liste de tuiles triées par couleur.
        //

        /// <summary>
        /// Fonction qui sert à trier les tuiles selon leur couleurs.
        /// </summary>
        /// <param name="pListeTuiles">Une liste de tuiles triée selon la couleur.</param>
        /// <returns></returns>
        public static List<Tuile> TrierTuilesCouleur(List<Tuile> pListeTuiles)
        {
            for (int i = 0; i < pListeTuiles.Count - 1; i++)
            {
                for (int j = 0; j < pListeTuiles.Count - i - 1; j++)
                {
                    if (pListeTuiles[j].Couleur.CompareTo(pListeTuiles[j + 1].Couleur) > 0)
                    {
                        Tuile tuileTriee = pListeTuiles[j];

                        pListeTuiles[j] = pListeTuiles[j + 1];

                        pListeTuiles[j + 1] = tuileTriee;
                    }
                }
            }
            return pListeTuiles;

        }



        //
        // TODO On doit obtenir le joueur suivant
        // À partir du joueur courant (MoteurJeu.JoueurCourant)
        //

        /// <summary>
        /// Fonction qui détermine le prochain joueur dans la file.
        /// </summary>
        public static void DeterminerJoueurSuivant()
        {

            while (MoteurJeu.FileJoueurs.Peek() != MoteurJeu.JoueurCourant)
            {
                MoteurJeu.FileJoueurs.Enqueue(MoteurJeu.FileJoueurs.Dequeue());
            }


            MoteurJeu.FileJoueurs.Enqueue(MoteurJeu.FileJoueurs.Dequeue());
            MoteurJeu.JoueurCourant = MoteurJeu.FileJoueurs.Peek();
        }

        ////
        //// TODO Écrire dans le fichier des gagnants le gagnant de la partie jouée.
        //// On doit écrire dans un fichier les informations de la partie qui vient d'être jouée dans le fichier des gagnants. Les informations reçues en paramètre
        //// doivent correspondre à l'objet que vous avez défini dans le fichier Gagnant.xaml.cs.
        ////
        //// Le fichier doit être nommé gagnants.dat.
        ////
        /// <summary>
        /// Fonction qui sert à écrire une liste de joueurs gagnants
        /// </summary>
        /// <param name="pListeGagnants">Une liste de joueurs gagnants</param>
        public static void EcrireGagnants(List<Tuple<DateTime, string, string, int>> pListeGagnants)
        {
            using StreamWriter swGagnants = new StreamWriter("gagnants.dat");
            swGagnants.Write(JsonSerializer.Serialize(pListeGagnants, typeof(List<Tuple<DateTime, string, string, int>>)));
        }




    }
   
}


