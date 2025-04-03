namespace Rummikub.Models
{
    public class Joueur
    {
        public string NomUtilisateur { get; set; }
        public byte[] MotDePasse { get; set; }
        public LinkedList<DateTime> DernieresConnexions { get; set; }

        public bool EstHumain { get; set; }
        public bool EstDemarre { get; set; }
        public string Personnage { get; set; }
        public List<Tuile> TuilesEnMain { get; set; }
        
        //
        // Constructeur vide uniquement pour les besoins de la sérialisation.
        //
        public Joueur()
        {
        }

        public Joueur(string pNomUtilisateur, string pPersonnage)
        {
            NomUtilisateur = pNomUtilisateur;

            EstHumain = false;
            EstDemarre = false;
            Personnage = pPersonnage;
            TuilesEnMain = new List<Tuile>();
        }

        public Joueur(string pNomUtilisateur, byte[] pMotDePasse)
        {
            NomUtilisateur = pNomUtilisateur;
            MotDePasse = pMotDePasse;
            DernieresConnexions = new LinkedList<DateTime>();

            EstHumain = true;
            EstDemarre = false;
            TuilesEnMain = new List<Tuile>();
        }
    }
}