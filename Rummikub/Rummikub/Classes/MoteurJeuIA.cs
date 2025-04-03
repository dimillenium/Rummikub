using Rummikub.Classes;
using System.Windows.Controls;

namespace Rummikub.Models
{
    public static partial class MoteurJeu
    {
        public static List<Tuile> TourOrdinateur()
        {
            //
            // Tour de l'ORDINATEUR
            //
            // L'atout ne sera pas traité
            // C'est trop compliqué comme IA...
            //

            if (!JoueurCourant.EstDemarre)
            {
                List<Tuile> listeTemporaire;
               
                foreach (Tuile tuileCourante in JoueurCourant.TuilesEnMain)
                {
                    listeTemporaire = JoueurCourant.TuilesEnMain.Where(t => (tuileCourante.Valeur == t.Valeur - 1 || tuileCourante.Valeur == t.Valeur || tuileCourante.Valeur == t.Valeur + 1) && tuileCourante.Couleur == t.Couleur).ToList();
                    if (listeTemporaire.GroupBy(t => t.Valeur).Count() == listeTemporaire.Count && Fonctions.EstCoupPourDebuter(listeTemporaire))
                    {
                        JoueurCourant.TuilesEnMain = JoueurCourant.TuilesEnMain.Except(listeTemporaire).ToList();

                        return listeTemporaire.OrderBy(t => t.Valeur).ThenBy(t => t.Couleur).ToList();
                    }

                    listeTemporaire = JoueurCourant.TuilesEnMain.Where(t => (tuileCourante.Couleur == t.Couleur - 1 || tuileCourante.Couleur == t.Couleur || tuileCourante.Couleur == t.Couleur + 1) && tuileCourante.Valeur == t.Valeur).ToList();
                    if (listeTemporaire.GroupBy(t => t.Couleur).Count() == listeTemporaire.Count && Fonctions.EstCoupPourDebuter(listeTemporaire))
                    {
                        JoueurCourant.TuilesEnMain = JoueurCourant.TuilesEnMain.Except(listeTemporaire).ToList();

                        return listeTemporaire.OrderBy(t => t.Valeur).ThenBy(t => t.Couleur).ToList();
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<StackPanel, List<Tuile>> serie in SeriesSurJeu.Where(s => !s.Key.Name.Equals("stackPanelChevalet") && s.Value.Count > 0))
                {
                    foreach (Tuile tuileCourante in JoueurCourant.TuilesEnMain)
                    {
                        //
                        // Vérification pour une série
                        //
                        if ((tuileCourante.Valeur == serie.Value.First().Valeur - 1 || tuileCourante.Valeur == serie.Value.Last().Valeur + 1) && serie.Value.GroupBy(t => t.Couleur).Count() == 1 && tuileCourante.Couleur == serie.Value.First().Couleur)
                        {
                            JoueurCourant.TuilesEnMain.Remove(tuileCourante);
                            GererDeplacementTuile(serie.Key, tuileCourante, false);

                            return new List<Tuile> { tuileCourante };
                        }

                        //
                        // Vérification pour un groupe
                        //
                        if (tuileCourante.Valeur == serie.Value.First().Valeur && serie.Value.GroupBy(t => t.Couleur).Count() == serie.Value.Count && serie.Value.FindAll(t => t.Couleur == tuileCourante.Couleur).Count == 0)
                        {
                            JoueurCourant.TuilesEnMain.Remove(tuileCourante);
                            GererDeplacementTuile(serie.Key, tuileCourante, false);

                            return new List<Tuile> { tuileCourante };
                        }
                    }
                }
            }

            //
            // Si on est rendu ici, c'est que le joueur n'a pas été capable de jouer son coup pour
            // démarrer; il doit donc piocher une tuile
            //
            Tuile tuilePiochee = PiocherTuile();
            JoueurCourant.TuilesEnMain.Add(tuilePiochee);

            return new List<Tuile>();
        }
    }
}
