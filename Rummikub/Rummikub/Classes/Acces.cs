using Konscious.Security.Cryptography;
using Rummikub.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace Rummikub.Classes
{
    public static partial class Acces
    {
        //
        // TODO Créer les dictionnaires.
        // Les deux dictionnaires statiques nécessaires doivent être nommés _dicoJoueurs et _dicoSalts. Attention au niveau d'accès.
        //
          private static  Dictionary<string,Joueur> _dicoJoueurs = new Dictionary<string,Joueur>();
          private static  Dictionary<string, byte[]> _dicoSalts = new Dictionary<string, byte[]>();
        //
        // TODO Création du salt.
        //
        // La fonction retourne le salt généré.
        //
        /// <summary>
        /// Fonction qui créer un Salt
        /// </summary>
        /// <returns>un array de byte</returns>
        public static byte[] CreerSALT()
        {
            byte[] buffer = new byte[16];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            return buffer;

        }

        //
        // TODO Hacher une chaîne de caractères.
        // À partir du paramètre pChaine, on hache ce paramètre à l'aide du salt.
        //
        // La fonction retourne la chaîne de caractère hachée.
        //

        /// <summary>
        /// Fonction qui hache un mot de passe.
        /// </summary>
        /// <param name="pChaine"></param>
        /// <param name="pSalt"></param>
        /// <returns>Des bytes qui représente le mot de passe haché</returns>
        public static byte[] HacherMotDePasse(string pChaine, byte[] pSalt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(pChaine))
            {
                Salt = pSalt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 1024 * 1024
            };

            return argon2.GetBytes(16);

        }

        //
        // TODO Vérifie si une chaîne de carcatères correspond au mot de passe.
        // On vérifie si la chaîne de caractères pChaine, lorsqu'une fois hachée à partir du salt, correspond au mot de passe haché pMotDePasseDicoUtilisateur.
        //
        // La fonction retourne:
        // - true, s'il y a correspondance
        // - false, sinon.
        //

        /// <summary>
        /// Fonction qui vérifie sur le mot de passe correspond à celui d'un utilisateur. 
        /// </summary>
        /// <param name="pChaine"></param>
        /// <param name="pSalt"></param>
        /// <param name="pMotDePasseDicoUtilisateur"></param>
        /// <returns>Un mot de passe qui va être valider s'il correspond ou non.</returns>
        public static bool EstMotDePasseCorrespond(string pChaine, byte[] pSalt, byte[] pMotDePasseDicoUtilisateur)
        {
            return pMotDePasseDicoUtilisateur.SequenceEqual(HacherMotDePasse(pChaine, pSalt));
        }

        //
        // TODO Vérifie les informations de connexion avec les dictionnaires.
        // À partir de pNomUtilisateur et pMotDePasse, si l'utilisateur existe dans le dictionnaire _dicoJoueurs. Si c'est le cas, on doit vérifier la correspondance du mot de passe
        // par l'appel de la procédure EstMotDePasseCorrespond.
        //
        // La fonction retourne:
        // - true, si l'utilisateur est existant et que son mot de passe correspond
        // - false, sinon.
        //

        /// <summary>
        /// Fonction qui vérifie si les informations pour une connexion sont valides.
        /// </summary>
        /// <param name="pNomUtilisateur"></param>
        /// <param name="pMotDePasse"></param>
        /// <returns>Un booléen qui détermine si les informations sont valides ou non.</returns>
        public static bool EstInformationsConnexionValides(string pNomUtilisateur, string pMotDePasse)
        {
            string monUtilisateur = pNomUtilisateur.Trim();

            string MDP = pMotDePasse.Trim();

            if (_dicoJoueurs.ContainsKey(monUtilisateur))
            {
                 
                if (EstMotDePasseCorrespond(MDP, _dicoSalts[monUtilisateur], _dicoJoueurs[monUtilisateur].MotDePasse))
                {
                    return true;
                }
            }
            
                return false;
           
        }

        //
        // TODO Vérifie si le compte est existant, et création du compte.
        // On vérifie à partir des paramètres si l'utilisateur existe dans le dictionnaire _dicoJoueurs. Si ce n'est pas le cas, on crée une nouvelle instance Joueur en prenant soin 
        // de hacher correctement le mot de passe à partir de HacherMotDePasse. On ajoute les informations pertinentes aux dictionnaires.
        // 
        // La fonction retourne:
        // - true, si l'utilisateur a été créé et ajouté aux dictionnaires
        // - false, si l'utilisateur existe dans le dictionnaire
        //

        /// <summary>
        /// Fonction qui vérifie si un utilisateur est créer ou non.
        /// </summary>
        /// <param name="pNomUtilisateur"></param>
        /// <param name="pMotDePasse"></param>
        /// <returns>Un booléen qui confirme si l'utilisateur a déjà été créé ou non.</returns>
        public static bool EstUtilisateurCree(string pNomUtilisateur, string pMotDePasse)
        {
            bool estCree;

            pNomUtilisateur = pNomUtilisateur.Trim();

            if (_dicoJoueurs.ContainsKey(pNomUtilisateur))
            {
                
                estCree = false;
            }
            else
            {
                byte[] salt = CreerSALT();
                _dicoSalts.Add(pNomUtilisateur, salt);
                Joueur joueur = new Joueur(pNomUtilisateur,salt);

                joueur.NomUtilisateur = pNomUtilisateur;
                joueur.MotDePasse = HacherMotDePasse(pMotDePasse.Trim(), salt);
                _dicoJoueurs.Add(joueur.NomUtilisateur, joueur);
                estCree = true;
            }

            return estCree;
        }

        //
        // TODO Obtenir les informations de l'utilisateur à partir du dictionnaire
        // On doit rechercher le joueur dans le dictionnaire _dicoJoueurs à partir du paramètre pNomUtilisateur. À cette étape, soit l'utilisateur vient d'être créé ou soit
        // il était déjà existant dans le fichier users.dat.
        //
        // Dès que le joueur est obtenu, on doit initialiser son personnage à partir du paramètre pPersonnage. De plus, on doit également ajouter à la liste chaînée DernieresConnexions
        // la date/heure du jour à la première position de la liste.
        //
        // La fonction retourne l'objet Joueur obtenu à partir du dictionnaire.
        //

        /// <summary>
        /// Fonction qui obtient un utilisateur dans un dictionnaire.
        /// </summary>
        /// <param name="pNomUtilisateur"></param>
        /// <param name="pPersonnage"></param>
        /// <returns>Le joueur d'un utilisateur</returns>
        /// <exception cref="KeyNotFoundException">Si la clé est trouvée ou non.</exception>
        public static Joueur ObtenirUtilisateur(string pNomUtilisateur, string pPersonnage)
        {
            if (_dicoJoueurs.ContainsKey(pNomUtilisateur))
            {
                Joueur joueur = _dicoJoueurs[pNomUtilisateur];

                joueur.Personnage = pPersonnage;

                joueur.DernieresConnexions = new LinkedList<DateTime>();
                joueur.DernieresConnexions.AddFirst(DateTime.Now);


                return joueur;
            }
            else
            {
                throw new KeyNotFoundException("L'utilisateur n'existe pas dans le dictionnaire _dicoJoueurs.");
                
            }
        }

        //
        // TODO Écriture des fichiers des dictionnaires.
        // À partir des dictionnaires _dicoJoueurs et _dicoSalts, on écrit les données dans les fichiers.
        // 
        // Le fichier du dictionnaire _dicoJoueurs doit être nommé users.dat.
        // Le fichier du dictionnaire _dicoSalts doit être nommé users.slt.
        //
        // Les propriétés importantes à écrire sont NomUtilisateur, MotDePasse et DernieresConnexions. Les autres propriétés de la classe Joueur seront écrites dans le fichier, 
        // mais leurs valeurs importe peu.
        //

        /// <summary>
        /// Fonction qui écrit dans des dictionnaires pour les utilisateurs.
        /// </summary>
        public static void EcrireDictionnaires()
        {
            using StreamWriter swUsers = new StreamWriter("users.dat");
            swUsers.Write(JsonSerializer.Serialize(_dicoJoueurs, typeof(Dictionary<string, Joueur>)));

            using StreamWriter swSalt = new StreamWriter("users.slt");
            swSalt.Write(JsonSerializer.Serialize(_dicoSalts, typeof(Dictionary<string, byte[]>)));

        }

        //
        // TODO Lecture des fichiers des dictionnaires.
        // À partir des fichiers users.dat et users.slt, on lit les données, et on alimente les dictionnaires correspondants.
        //

        /// <summary>
        /// Fonction qui lit des informations dans des dictionnaires de données.
        /// </summary>
        public static void LireDictionnaires()
        {
            if(File.Exists("users.dat")&& File.Exists("users.slt"))
            {
                using StreamReader sr1 = new StreamReader("users.dat");
                {
                    _dicoJoueurs = JsonSerializer.Deserialize(
                        sr1.ReadToEnd(),typeof(Dictionary<string,Joueur>)
                        ) as Dictionary<string, Joueur>;

                }


                using StreamReader sr2 = new StreamReader("users.slt");
                {
                    _dicoSalts = JsonSerializer.Deserialize(
                        sr2.ReadToEnd(), typeof(Dictionary<string, byte[]>)
                        ) as Dictionary<string, byte[]>;

                }
            }
            
        }
    }

}
