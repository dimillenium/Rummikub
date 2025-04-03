using Rummikub.Classes;
using Rummikub.Models;
using System.Windows;

namespace Rummikub.Views
{
    public partial class Gagnant : Window
    {
        //
        // TODO Définir ici un objet qui contiendra les informations suivantes des joueurs:
        // - Date de la partie jouée
        // - Le nom du joueur
        // - Le personnage
        // - Le pointage du joueur
        //
        // Votre objet devra pouvoir être trié.
        //
        // Ces informations seront écrites dans un fichier à partir de Fonctions.EcrireGagnants.
        //  

        public  List<Tuple<DateTime, string, string, int>> joueursInfos; 


        /// <summary>
        /// Fonction qui met les informations sur le gagnant d'une partie.
        /// </summary>
        /// <param name="pRaisonGagnant">La raison de la victoire d'un gagnant</param>
        public Gagnant(string pRaisonGagnant)
        {

            InitializeComponent();

            Utils.JouerGagnant();

            DeterminerGagnant();

            Utils.Trace($"Il y a un gagnant, car {pRaisonGagnant}.", Utils.CategorieTrace.INFO);

            //
            // TODO Remplacer le texte GAGNANT par le nom du gagnant de la partie.
            //


            txtRaison.Text = $"{pRaisonGagnant}, et le gagnant de la partie est {joueursInfos[0].Item2}. Voici les résultats :";

            //
            // TODO Concaténer votre objet avec les données sauvegardées des précédentes parties jouées.
            // Utiliser la procédure Fonctions.LireGagnants() afin d'alimenter votre objet déclaré ci-haut. Attention, les données de la partie jouée doit être inscrite
            // avant les parties précédentes.
            ////
            List<Tuple<DateTime, string, string, int>> donneesPrecedentes = Fonctions.LireGagnants();

            // Concaténer les données lues avec les données actuelles
            if (donneesPrecedentes != null)
            {
                
                donneesPrecedentes.AddRange(joueursInfos);

               
                joueursInfos = donneesPrecedentes;
            }
            else
            {
                MessageBox.Show("Rien a l'interieur");
            }
            AfficherAutresParties();


            //
            // TODO Appeler Fonctions.EcrireGagnants avec l'objet qui contient les informations des joueurs de cette partie, mais aussi des parties précédentes.

           Fonctions.EcrireGagnants(joueursInfos);
            
     
        }

        //
        // TODO Déterminer le gagnant de la partie jouée.
        // À partir des tuiles restantes pour chaque joueur, on doit déterminer qui est le gagnant de la partie. Pour ce faire, peu importe la couleur, les valeurs des
        // tuiles restantes (voir Joueur.TuilesEnMain) sont tout simplement additionnées. Attention, ici, la valeur d'un atout est de 30.
        //
        // Le gagnant est le joueur qui possède le pointage le moins élevé. L'objet que vous aurez créé plus haut doit contenir les joueur dans l'ordre du pointage, du
        // moins élevé au plus élevé.
        //
        // Pour afficher les résultats de la partie, il faut affecter une liste (IEnumerable) à l'item itemsGagnants.ItemsSource. Cet item prend un tuple à deux valeurs:
        // - la première valeur sera l'image du personnage passé par la chaîne "/Resources/Images/PersonnageDuJoueur.gif" 
        // - la deuxième valeur est le texte affiché à l'écran, qui ressemble à "Le joueur NomDuJoueur termine la partie avec PointageDuJoueur points"
        //

        /// <summary>
        /// Fonction qui détermine qui est la gagnant.
        /// </summary>
        private void DeterminerGagnant()
        {
            Queue<Joueur> joueursTemp = new Queue<Joueur>(MoteurJeu.FileJoueurs);

            joueursInfos = new List<Tuple<DateTime, string, string, int>> { };

            while (joueursTemp.Count > 0)
            {
                Joueur j = joueursTemp.Dequeue();

                int pointageTotal = 0;

                foreach (Tuile tuile in j.TuilesEnMain)
                {
                    if (tuile is Atout)
                    {
                        pointageTotal += 30;
                    }
                    else
                    {
                        pointageTotal += tuile.Valeur;
                    }
                }

                joueursInfos.Add(new Tuple<DateTime, string, string, int>(DateTime.Now, j.NomUtilisateur, j.Personnage, pointageTotal));
            }

            for (int i = 0; i < joueursInfos.Count - 1; i++)
            {
                for (int j = i + 1; j < joueursInfos.Count; j++)
                {
                    if (joueursInfos[i].Item4 > joueursInfos[j].Item4)
                    {
                        var temp = joueursInfos[i];
                        joueursInfos[i] = joueursInfos[j];
                        joueursInfos[j] = temp;
                    }
                }
            }

            List<Tuple<string, string>> Gagnants = new List<Tuple<string, string>>();


            foreach (var infoJoueur in joueursInfos)
            {
                string imagePersonnage = $"/Resources/Images/{infoJoueur.Item3}.gif";
                string message = $"Le joueur {infoJoueur.Item2} termine la partie avec {infoJoueur.Item4} points";

                Gagnants.Add(new Tuple<string,string>(imagePersonnage,message));
            }


            itemsGagnants.ItemsSource = Gagnants;




        }

        //
        // TODO Afficher les résultats des anciennes parties jouées.
        // À partir de votre objet défini et renseigné à partir de Fonctions.LireGagnants(), les informations des parties précédemment jouées doivent être affichées
        // à l'écran. Pour ce faire, il faut utiliser la procédure lstBoxAutresGagnants.Items.Add(...) pour chacun des éléments.
        //

        /// <summary>
        /// Fonction qui sert à afficher les informations sur les autres parties.
        /// </summary>
        private void AfficherAutresParties()
        {
            if (joueursInfos!= null && joueursInfos.Count > 0)
            {

                for (int i = 0; i < joueursInfos.Count-4; i++)
                {
                    var gagnant = joueursInfos[i];
                    lstBoxAutresGagnants.Items.Add($"Date : {gagnant.Item1}, Nom : {gagnant.Item2}, Personnage : {gagnant.Item3}, Score : {gagnant.Item4}");
                }
            }
            else
            {
               
                lstBoxAutresGagnants.Items.Add("Aucune partie jouée précédemment.");
            }
        }
    }
}
