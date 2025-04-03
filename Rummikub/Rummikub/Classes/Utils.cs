using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace Rummikub.Classes
{
    public static class Utils
    {
        public enum CategorieTrace
        {
            INFO,
            AVERTISSEMENT,
            ERREUR
        }

        public static void JouerChanson()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();

            player.SoundLocation = "Resources/Sounds/song.wav";
            player.PlayLooping();
        }

        public static void InitialisationTrace(string pNomFichier)
        {
            Stream fichierTrace = File.Create(pNomFichier);

            InitialisationListener(fichierTrace);
        }

        public static void InitialisationTrace(string pNomRepertoire, string pNomFichier)
        {
            Directory.CreateDirectory(pNomRepertoire);
            Stream fichierTrace = File.Create(pNomRepertoire + pNomFichier);

            InitialisationListener(fichierTrace);
        }

        public static void InitialisationListener(Stream pStreamTrace)
        {
            TextWriterTraceListener listenerTrace = new TextWriterTraceListener(pStreamTrace);
            System.Diagnostics.Trace.AutoFlush = true;
            System.Diagnostics.Trace.Listeners.Add(listenerTrace);
        }

        public static void Trace(string pLigne, CategorieTrace pCategorie)
        {
            System.Diagnostics.Trace.WriteLine(pLigne, pCategorie.ToString());
        }

        public static void Trace(string pLigne, CategorieTrace pCategorie, TextBox pControle)
        {
            Trace(pLigne, pCategorie);

            pControle.Text += "> " + pLigne + "\n";
            pControle.ScrollToEnd();
        }

        public static double FacteurPlusPres(double pValue, double pFactor)
        {
            return Math.Round(pValue / pFactor, MidpointRounding.AwayFromZero) * pFactor;
        }

        public static void JouerGagnant()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();

            player.SoundLocation = "Resources/Sounds/gagnant.wav";
            player.Play();
        }
    }
}
