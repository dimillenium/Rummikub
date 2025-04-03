using System.ComponentModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rummikub.Models
{
    public abstract class Tuile
    {
        public enum Couleurs
        {
            [Description("NOIRE")]
            BLACK,

            [Description("JAUNE")]
            DARKGOLDENROD,

            [Description("BLEUE")]
            BLUE,

            [Description("ROUGE")]
            RED
        }

        private Guid _id;
        public StackPanel Emplacement { get; set; }
        public int Valeur { get; set; }
        public Couleurs Couleur { get; set; }

        protected Tuile()
        {
            _id = Guid.NewGuid();
        }

        protected Tuile(int pValeur, Couleurs pCouleur) : this()
        {
            Valeur = pValeur;
            Couleur = pCouleur;
        }

        public Guid ID { get { return _id; } }

        public Brush CouleurB()
        {
            return (Brush)new BrushConverter().ConvertFrom(Couleur.ToString());
        }

        public string CouleurS()
        {
            FieldInfo fi = Couleur.GetType().GetField(Couleur.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return Couleur.ToString();
        }

        public int CompareTo(Tuile autre)
        {
            if (this.Valeur < autre.Valeur)
            {
                return -1;
            }
            else if(this.Valeur > autre.Valeur) 
            { 

                return 1;
            }
            else
            {
                return 0;
            }


        }
    }
}