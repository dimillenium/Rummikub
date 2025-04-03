using Rummikub.Models;
using System.IO;
using System.Numerics;
using System.Text.Json;
using System.Windows.Controls;
using static Rummikub.Models.Tuile;

namespace Rummikub.Classes
{
    public static partial class Fonctions
    {

        ////
        //// TODO Obtenir les informations du joueur humain à partir de la file des joueurs.
        //// À partir de MoteurJeu.FileJoueurs, on doit retourner l'objet Joueur qui correspond à l'humain.
        ////
        //// La fonction retourne l'objet Joueur trouvé.
        ////
        
        /// <summary>
        /// Fonction qui permet de retourner le joueur de type humain
        /// </summary>
        /// <returns>Joueur de type humain</returns>
        /// <exception cref="InvalidOperationException">Exception lancé si aucun joueur de type humain trouvé</exception>
        public static Joueur ObtenirJoueurHumain()
        {
            Joueur Joueur = new Joueur();
            for (int i = 0; i < MoteurJeu.NB_JOUEURS; i++)
            {
                Joueur j = MoteurJeu.FileJoueurs.Dequeue();

                
                if (j.EstHumain)
                {
                    MoteurJeu.FileJoueurs.Enqueue(j);
                    Joueur = j;
                }
                else
                {
                    
                    MoteurJeu.FileJoueurs.Enqueue(j);
                }
            }
             return Joueur;
            throw new InvalidOperationException("Aucun Joueur humain trouvé dans la file des joueurs.");
        }

        

        ////
        //// TODO On détermine quel joueur débutera la partie, et affecter le joueur courant.
        //// On doit créer une liste temporaire de tuiles qui possèdent toute les valeurs possibles d'une tuile (MoteurJeu.NB_VALEUR), peu importe la couleur. On doit
        //// simuler que chaque joueur de la partie (il y en a 4) pige une tuile dans cette liste, de façon aléatoire.
        ////
        //// Le joueur qui possède la tuile de la plus grande valeur débutera la partie, et on affectera ce joueur à la propriét MoteurJeu.JoueurCourant.
        ////
        //// La fonction retourne un array de la valeur des tuiles pigées. Le joueur 1 (qui correspond à l'humain) est l'index 0.
        ////


        /// <summary>
        /// Fonction qui permet de determiner comment les joueurs debutent la partie
        /// </summary>
        /// <returns>vecteurs de tuiles aléatoires</returns>
        public static int[] DeterminerOrdreJoueurs()
        {
            Random random = new Random();
            List<Tuile> tuilesPossibles = new List<Tuile>();


            for (int valeur = 1; valeur <= MoteurJeu.NB_VALEUR; valeur++)
            {
                tuilesPossibles.Add(new Normale(valeur, Tuile.Couleurs.BLUE));

            }


            int[] valeursPiochees = new int[MoteurJeu.NB_JOUEURS];


            for (int joueurIndex = 0; joueurIndex < MoteurJeu.NB_JOUEURS; joueurIndex++)
            {
               MoteurJeu.FileJoueurs.Enqueue(MoteurJeu.FileJoueurs.Dequeue());
                Joueur joueur = MoteurJeu.FileJoueurs.Peek();

                int indexPioche = random.Next(tuilesPossibles.Count);
                valeursPiochees[joueurIndex] = tuilesPossibles[indexPioche].Valeur;
                tuilesPossibles.RemoveAt(indexPioche);
            }



            int valeurMax = 0;


            for (int i = 0; i < valeursPiochees.Length; i++)
            {
                if (valeursPiochees[valeurMax] < valeursPiochees[i])
                {
                    valeurMax = i;


                }
            }
            for (int i = 0; i < MoteurJeu.FileJoueurs.Count; i++)
            {
                if (i == valeurMax)
                {
                    MoteurJeu.JoueurCourant = MoteurJeu.FileJoueurs.Peek();
                }
                MoteurJeu.FileJoueurs.Enqueue(MoteurJeu.FileJoueurs.Dequeue());
            }


            return valeursPiochees;



        }


        ////
        //// TODO On évalue si un joueur peut débuter le jeu.
        //// À partir de la liste de tuiles passées en paramètre, on doit évaluer si la valeur de cette liste est suffisante pour débuter le jeu.
        ////
        //// Deux vérifications doivent être faites:
        //// - Est-ce que la suite possède le nombre de tuile minimum (MoteurJeu.NB_TUILE_PAR_SERIE_MINIMUM) pour une suite 
        //// - Est-ce que la valeur totale des tuiles est égale ou supérieure à la valeur nécessaire pour débuter (MoteurJeu.TOTAL_POUR_DEMARRAGE)
        ////
        //// La fonction retourne:
        //// - true, si le joueur peut débuter
        //// - false, sinon.
        ////
        
        
        /// <summary>
        /// Fonction qui verifie si les conditions sont remplies pour débuter le jeu
        /// </summary>
        /// <param name="pListeTuiles">Liste de tuiles</param>
        /// <returns> true ou false</returns>

        public static bool EstCoupPourDebuter(List<Tuile> pListeTuiles)
        {
            if( pListeTuiles.Count >= MoteurJeu.NB_TUILE_PAR_SERIE_MINIMUM)
            {
                int valeurTotale = 0;

                foreach(Tuile t in pListeTuiles)
                {
                    valeurTotale += t.Valeur;
                }

                if( valeurTotale > MoteurJeu.TOTAL_POUR_DEMARRAGE)
                {
                    return true;
                }

            }

            return false;

        }

        ////
        //// TODO Dépiler la tuile pour revenir sur le dernier coup.
        //// On doit dépiler la tuile se trouvant au sommet de la pile (voir propriétés de MoteurJeu).
        ////
        //// La fonction doit retourner un tuple<StackPanel, Tuile>.
        
        /// <summary>
        /// Fonction qui dépile la tuile qui se trouve au sommet d'une pile
        /// </summary>
        /// <returns>Tuple<StackPanel,Tuile></returns>
        public static Tuple<StackPanel, Tuile> DepilerDeplacement()
        {
            Tuple <StackPanel,Tuile> tuileDepile = MoteurJeu.PileDeplacements.Pop();

            return tuileDepile;

        }

        


        ////
        //// TODO On doit trier (ou grouper) une liste de tuiles passées en paramètre par valeur.
        //// À partir de cette liste, toutes les tuiles sont triées par valeur, de la plus petite à la plus grande valeur. L'ordre des couleurs n'importe pas, tout comme l'atout.
        ////
        //// La fonction retourne la liste de tuiles triées par valeur.
        ////

        /// <summary>
        /// Fonction qui permet de trier les tuiles en ordre décroissant
        /// </summary>
        /// <param name="pListeTuiles"></param>
        /// <returns></returns>
        public static List<Tuile> TrierTuilesValeur(List<Tuile> pListeTuiles)
        {
            for(int i = 0;i<pListeTuiles.Count -1;i++) 
            {
                for(int j = 0; j < pListeTuiles.Count-i-1;j++)
                {
                    if (pListeTuiles[j].CompareTo(pListeTuiles[j + 1])>0 )
                    {
                        Tuile temp  = pListeTuiles[j];

                        pListeTuiles[j] = pListeTuiles[j+1];

                        pListeTuiles[j+1] = temp;
                    }
                }   
            }
            return pListeTuiles;
        }


        ////
        //// TODO On évalue s'il y a un gagnant parmi les joueurs en retournant si c'est le cas, la raison de la fin de la partie.
        ////
        //// La fonction retourne une chaîne de caractère selon la situation suivante:
        //// - si le sac (MoteurJeu.Sac) est vide, on retourne la chaîne "Le sac est vide"
        //// - si un joueur ne possède plus de tuiles en main, on retourne la chaîne "(Nom de l'utilisateur) a vidé son chevalet"
        //// - sinon, la partie n'est pas terminée et on retourne une chaîne vide.
        
        /// <summary>
        /// Fonction qui permet d'évaluer un gagnant parmi joueurs en fonction des cas
        /// </summary>
        /// <returns>string</returns>
        public static string AnalyserSituationsGagnant()
        {
            Queue<Joueur> fileTemporaire = new Queue<Joueur>();

            if (MoteurJeu.Sac.Count <= 0)
            {
                return "Le sac est vide";
            }

            while (MoteurJeu.FileJoueurs.Count > 0)
            {
                Joueur joueurCourant = MoteurJeu.FileJoueurs.Dequeue();

                if (joueurCourant.TuilesEnMain.Count <= 0)
                {
                    
                    return $"{joueurCourant.NomUtilisateur} a vidé son chevalet";
                }
                else
                {
                    
                    fileTemporaire.Enqueue(joueurCourant);
                }
            }

            while (fileTemporaire.Count > 0)
            {
                MoteurJeu.FileJoueurs.Enqueue(fileTemporaire.Dequeue());
            }



            return "";

        }

        ////
        //// TODO Lire le fichier des gagnants précédents.
        //// On doit lire les informations des parties précédemment jouées, écrites dans le fichier gagnants.dat.
        ////
        //// La fonction retourne le type de l'objet que vous avez défini dans le fichier Gagnant.xaml.cs.

        /// <summary>
        /// Fonction qui permet de lire le fichier gagnants.dat dans lequel sont stockées les parties précédentes.
        /// </summary>
        /// <returns></returns>
        public static List<Tuple<DateTime, string, string, int>> LireGagnants()
        {
            List<Tuple<DateTime, string, string, int>> gagnants = new List<Tuple<DateTime, string, string, int>>() ;

            if (File.Exists("gagnants.dat"))
            {
                using (StreamReader sw1 = new StreamReader("gagnants.dat"))
                {
                    string contenu = sw1.ReadToEnd();
                    if (contenu != null)
                    {
                        gagnants = JsonSerializer.Deserialize<List<Tuple<DateTime, string, string, int>>>(contenu);
                    }

                    return gagnants;
                }
               
            }
            return null;
        }
    }
}
