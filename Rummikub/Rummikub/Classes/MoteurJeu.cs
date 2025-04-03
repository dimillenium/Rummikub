using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using Rummikub.Classes;

namespace Rummikub.Models
{
    public static partial class MoteurJeu
    {
        public const int NB_JOUEURS = 4;
        public const int NB_COULEURS = 4;
        public const int NB_VALEUR = 13;

        public const int NB_TUILE_ATOUT = 2;
        public const int NB_TUILE_DEMARRAGE = 14;
        public const int NB_TUILE_PAR_SERIE_MINIMUM = 3;

        public const int TOTAL_POUR_DEMARRAGE = 30;

        public static Queue<Joueur> FileJoueurs { get; set; } = new Queue<Joueur>();
        public static Joueur JoueurCourant { get; set; }

        public static List<Tuile> JeuComplet { get; set; }
        public static List<Tuile> Sac { get; set; }

        public static Stack<Tuple<StackPanel, Tuile>> PileDeplacements = new Stack<Tuple<StackPanel, Tuile>>();
        public static Dictionary<StackPanel, List<Tuile>> SeriesSurJeu = new Dictionary<StackPanel, List<Tuile>>();

        public static void PreparerJeu()
        {
            JeuComplet = new List<Tuile>();
            Sac = new List<Tuile>();

            for (int i = 1; i <= NB_VALEUR; i++)
            {
                for (int j = 0; j <= NB_COULEURS - 1; j++)
                {
                    //
                    // Il y a 2 tuiles de même valeur par couleur
                    //
                    JeuComplet.Add(new Normale(i, (Tuile.Couleurs)j));
                    JeuComplet.Add(new Normale(i, (Tuile.Couleurs)j));
                }
            }

            for (int k = 1; k <= NB_TUILE_ATOUT; k++)
                JeuComplet.Add(new Atout());

            //
            // Se conserver une copie complète du jeu pour la recherche des tuiles
            //
            Sac = JeuComplet.ToList();
        }

        public static void DistribuerTuiles()
        {
            for (int i = 0; i < NB_JOUEURS; i++)
            {
                FileJoueurs.ElementAt(i).TuilesEnMain = new List<Tuile>();

                for (int j = 1; j <= NB_TUILE_DEMARRAGE; j++)
                {
                    //
                    // Vu que les tuile sont déjà mélangées, on choisi toujours la première tuile
                    //
                    FileJoueurs.ElementAt(i).TuilesEnMain.Add(Sac[0]);
                    Sac.RemoveAt(0);
                }
            }
        }

        public static Tuile PiocherTuile()
        {
            if (Sac.Count == 0)
                return null;

            Tuile tuilePiochee = Sac[0];

            Sac.RemoveAt(0);

            return tuilePiochee;
        }

        public static Tuile TrouverTuile(Guid pID)
        {
            return JeuComplet.Single(t => t.ID == pID);
        }

        public static bool EstSeriesValides()
        {
            bool estSuiteConforme;

            foreach (KeyValuePair<StackPanel, List<Tuile>> serie in SeriesSurJeu.Where(s => !s.Key.Name.Equals("stackPanelChevalet") && s.Value.Count > 0))
            {
                estSuiteConforme = true;

                if (serie.Value.Count < NB_TUILE_PAR_SERIE_MINIMUM || serie.Value.GroupBy(t => t.Valeur).Count() != serie.Value.Count || serie.Value.GroupBy(t => t.Couleur).Count() != 1)
                {
                    if (serie.Value.Count < NB_TUILE_PAR_SERIE_MINIMUM || serie.Value.GroupBy(t => t.Couleur).Count() != serie.Value.Count || serie.Value.GroupBy(t => t.Valeur).Count() != 1)
                        estSuiteConforme = false;
                }

                if (!estSuiteConforme)
                    return false;
            }

            return true;
        }

        public static bool EstPilePourDebuter()
        {
            List<Tuile> pileTuiles = new List<Tuile>();

            foreach (Tuple<StackPanel, Tuile> tuple in PileDeplacements)
                pileTuiles.Add(tuple.Item2);

            return Fonctions.EstCoupPourDebuter(pileTuiles);
        }

        public static void GererDeplacementTuile(StackPanel pStackPanelDestination, Tuile pTuile, bool pEstEmpilee)
        {
            if (pEstEmpilee)
                Fonctions.PilerDeplacement(Tuple.Create(pTuile.Emplacement, pTuile));

            //
            // On retire la tuile de son emplacement initiale
            //
            if (pTuile.Emplacement != null && SeriesSurJeu.ContainsKey(pTuile.Emplacement))
                SeriesSurJeu[pTuile.Emplacement].Remove(pTuile);

            //
            // On place la tuile à son nouvel emplacement
            //
            if (SeriesSurJeu.ContainsKey(pStackPanelDestination))
            {
                pTuile.Emplacement = pStackPanelDestination;
                SeriesSurJeu[pStackPanelDestination].Add(pTuile);
            }
            else
            {
                List<Tuile> tuile = new List<Tuile>();

                pTuile.Emplacement = pStackPanelDestination;
                tuile.Add(pTuile);
                SeriesSurJeu.Add(pStackPanelDestination, tuile);
            }

            //
            // On tri dans l'ordre uniquement pour les tuiles placées sur la table
            //
            if (!pStackPanelDestination.Name.Equals("stackPanelChevalet"))
                SeriesSurJeu[pStackPanelDestination] = SeriesSurJeu[pStackPanelDestination].OrderBy(v => v.Valeur).ThenBy(c => c.Couleur).ToList();
        }

        public static void GererDeplacementTuiles(StackPanel pStackPanelDestination, List<Tuile> pListeTuiles, bool pEstEmpilee)
        {
            foreach (Tuile t in pListeTuiles)
                GererDeplacementTuile(pStackPanelDestination, t, pEstEmpilee);
        }

    }
}
