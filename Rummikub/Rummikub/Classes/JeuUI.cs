using Rummikub.Classes;
using Rummikub.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rummikub.Views
{
    public partial class Jeu : Window
    {
        private Key? _toucheEnfoncee;
        private bool _estSerieEnDeplacement;
        private double _pointX, _pointY;
        private StackPanel _emplacementCourant;

        private void Jeu_Loaded(object pSender, RoutedEventArgs pEvent)
        {
            KeyDown += HandleKeyPress;
            KeyUp += HandleKeyUp;
        }

        private void HandleKeyPress(object pSender, KeyEventArgs pEvent)
        {
            _toucheEnfoncee = pEvent.Key;
        }

        private void HandleKeyUp(object pSender, KeyEventArgs pEvent)
        {
            _toucheEnfoncee = null;
        }

        private void Canvas_PreviewMouseUp(object pSender, MouseButtonEventArgs pEvent)
        {
            if (_toucheEnfoncee == Key.LeftCtrl && _tuilesSelectionneesPourDeplacement.Count > 0 && _emplacementCourant == null)
            {
                MoteurJeu.GererDeplacementTuiles(CreerStackPanel(0), _tuilesSelectionneesPourDeplacement, true);
                GererAffichage();
            }
        }

        private void StackPanel_MouseEnter(object pSender, MouseEventArgs pEvent)
        {
            (pSender as StackPanel).Background = Brushes.Yellow;
            _emplacementCourant = pSender as StackPanel;
        }

        private void StackPanel_MouseLeave(object pSender, MouseEventArgs pEvent)
        {
            (pSender as StackPanel).Background = Brushes.Transparent;
            _emplacementCourant = null;
        }

        private void StackPanel_PreviewMouseDown(object pSender, MouseButtonEventArgs pEvent)
        {
            if (_toucheEnfoncee == Key.LeftShift && !_estSerieEnDeplacement)
            {
                Point _positionStackPanelOrigine = (pSender as StackPanel).TransformToAncestor(canvasTableJeu).Transform(new Point(0, 0));

                _pointX = Mouse.GetPosition(canvasTableJeu).X - _positionStackPanelOrigine.X;
                _pointY = Mouse.GetPosition(canvasTableJeu).Y - _positionStackPanelOrigine.Y;

                (pSender as StackPanel).CaptureMouse();

                _estSerieEnDeplacement = true;
            }
        }

        private void StackPanel_PreviewMouseMove(object pSender, MouseEventArgs pEvent)
        {
            if (_toucheEnfoncee == Key.LeftShift && _estSerieEnDeplacement)
            {
                Canvas.SetLeft(pSender as StackPanel, Utils.FacteurPlusPres(Mouse.GetPosition(canvasTableJeu).X - _pointX, 25));
                Canvas.SetTop(pSender as StackPanel, Utils.FacteurPlusPres(Mouse.GetPosition(canvasTableJeu).Y - _pointY, 25));
            }
        }

        private void StackPanel_PreviewMouseUp(object pSender, MouseButtonEventArgs pEvent)
        {
            if (_toucheEnfoncee == Key.LeftShift && _estSerieEnDeplacement)
            {
                _estSerieEnDeplacement = false;
                (pSender as StackPanel).ReleaseMouseCapture();
            }

            if (_toucheEnfoncee == Key.LeftCtrl && _emplacementCourant != null)
            {
                if (!MoteurJeu.JoueurCourant.EstDemarre)
                {
                    Utils.Trace($"Afin de démarrer, {MoteurJeu.JoueurCourant.NomUtilisateur} doit jouer en un seul coup uniquement les tuiles de son chevalet", Utils.CategorieTrace.INFO, txtTrace);

                    return;
                }

                MoteurJeu.GererDeplacementTuiles(pSender as StackPanel, _tuilesSelectionneesPourDeplacement, true);
                GererAffichage();
            }
        }

        private void ToggleButton_Checked(object pSender, RoutedEventArgs pEvent)
        {
            if (_toucheEnfoncee == Key.LeftCtrl)
            {
                (pSender as ToggleButton).IsChecked = false;

                return;
            }

            if (!((pSender as ToggleButton).Parent as StackPanel).Name.Equals("stackPanelChevalet") && !MoteurJeu.JoueurCourant.EstDemarre)
            {
                Utils.Trace($"Afin de démarrer, {MoteurJeu.JoueurCourant.NomUtilisateur} doit jouer en un seul coup uniquement les tuiles de son chevalet", Utils.CategorieTrace.INFO, txtTrace);
                (pSender as ToggleButton).IsChecked = false;

                return;
            }

            Tuile tuile = MoteurJeu.TrouverTuile(Guid.Parse((pSender as ToggleButton).Uid));

            if (tuile is Atout)
            {
                ChoixAtout choixAtout = new ChoixAtout(tuile);

                choixAtout.ShowDialog();

                if (choixAtout.DialogResult.HasValue && choixAtout.DialogResult.Value)
                    Utils.Trace($"L'atout sera considéré comme {tuile.Valeur} {tuile.CouleurS()}", Utils.CategorieTrace.INFO, txtTrace);
            }

            _tuilesSelectionneesPourDeplacement.Add(tuile);
        }

        private void ToggleButton_Unchecked(object pSender, RoutedEventArgs pEvent)
        {
            _tuilesSelectionneesPourDeplacement.Remove(MoteurJeu.TrouverTuile(Guid.Parse((pSender as ToggleButton).Uid)));
        }

        private void btnTriParValeur_Click(object pSender, RoutedEventArgs pEvent)
        {           
            MoteurJeu.SeriesSurJeu[MoteurJeu.SeriesSurJeu.First().Key] = Fonctions.TrierTuilesValeur(MoteurJeu.SeriesSurJeu[MoteurJeu.SeriesSurJeu.First().Key]);
            GererAffichage();
        }

        private void btnTriParCouleur_Click(object pSender, RoutedEventArgs pEvent)
        {        
            MoteurJeu.SeriesSurJeu[MoteurJeu.SeriesSurJeu.First().Key] = Fonctions.TrierTuilesCouleur(MoteurJeu.SeriesSurJeu[MoteurJeu.SeriesSurJeu.First().Key]);
            GererAffichage();
        }

        private void btnJoueurSuivant_Click(object pSender, RoutedEventArgs pEvent)
        {
            if (EstGagnant())
                return;
            else
            {
                if (!MoteurJeu.EstSeriesValides())
                {
                    Utils.Trace($"Le jeu comporte des SÉRIES qui ne sont PAS CONFORMES", Utils.CategorieTrace.ERREUR, txtTrace);

                    return;
                }


                if (MoteurJeu.PileDeplacements.Count == 0)
                {
                    Utils.Trace($"Au moins un déplacement doit être effectué", Utils.CategorieTrace.INFO, txtTrace);

                    return;
                }


                if (!MoteurJeu.JoueurCourant.EstDemarre && MoteurJeu.EstPilePourDebuter())
                    MoteurJeu.JoueurCourant.EstDemarre = true;

                if (!MoteurJeu.JoueurCourant.EstDemarre && btnPiocherTuile.IsEnabled)
                    Utils.Trace($"Pour pouvoir démarrer, vous devez faire une ou des séries d'une valeur d'au moins {MoteurJeu.TOTAL_POUR_DEMARRAGE} ou piocher une nouvelle tuile", Utils.CategorieTrace.INFO, txtTrace);
                else
                    JoueurSuivant();
            }
        }

        private void btnPiocherTuile_Click(object pSender, RoutedEventArgs pEvent)
        {
            if (MoteurJeu.PileDeplacements.Count > 0)
            {
                Utils.Trace($"La pioche doit être votre seul coup. Vous devez dépiler avant de piocher.", Utils.CategorieTrace.INFO, txtTrace);

                return;
            }

            Tuile tuilePiochee = MoteurJeu.PiocherTuile();

            if (EstGagnant())
                return;

            Fonctions.ObtenirJoueurHumain().TuilesEnMain.Add(tuilePiochee);

            MoteurJeu.GererDeplacementTuile(FindName("stackPanelChevalet") as StackPanel, tuilePiochee, false);
            GererAffichage();

            if (tuilePiochee is Atout)
                Utils.Trace($"Tuile ATOUT piochée\n", Utils.CategorieTrace.INFO, txtTrace);
            else
                Utils.Trace($"Tuile {tuilePiochee.Valeur} {tuilePiochee.CouleurS()} piochée\n", Utils.CategorieTrace.INFO, txtTrace);

            btnPiocherTuile.IsEnabled = false;
            JoueurSuivant();
        }

        private void btnDepilerDeplacement_Click(object pSender, RoutedEventArgs pEvent)
        {
            Tuple<StackPanel, Tuile> tuile = Fonctions.DepilerDeplacement();

            _tuilesSelectionneesPourDeplacement.Clear();
            _tuilesSelectionneesPourDeplacement.Add(tuile.Item2);

            MoteurJeu.GererDeplacementTuiles(tuile.Item1, _tuilesSelectionneesPourDeplacement, false);
            GererAffichage();
        }

        private void AfficherJoueurs()
        {
            for (int i = 0; i < MoteurJeu.NB_JOUEURS; i++)
            {
                (FindName("imgPersonnage" + i) as Image).Source = new BitmapImage(new Uri("/Resources/Images/" + MoteurJeu.FileJoueurs.ElementAt(i).Personnage + ".gif", UriKind.Relative));
                (FindName("txtNomUtilisateur" + i) as TextBlock).Text = MoteurJeu.FileJoueurs.ElementAt(i).NomUtilisateur;

                ToggleButton tuileAffichage = new ToggleButton()
                {
                    Name = "tuileAffichage" + i,
                    Style = Resources["StyleTuileAffichage"] as Style,
                };
                RegisterName(tuileAffichage.Name, tuileAffichage);

                Grid.SetColumn(tuileAffichage, 0);
                Grid.SetRow(tuileAffichage, i == 0 ? 4 : i);

                gridJoueurs.Children.Add(tuileAffichage);
            }
        }

        private ToggleButton CreerTuile(Tuile pTuile)
        {
            ToggleButton nouvelleTuile = new ToggleButton
            {
                Uid = pTuile.ID.ToString(),
                Width = 50,
                Height = 70
            };

            if (pTuile is Normale)
            {
                nouvelleTuile.Content = pTuile.Valeur;
                nouvelleTuile.Foreground = pTuile.CouleurB();
                nouvelleTuile.Style = Resources["StyleTuileNormale"] as Style;
            }
            else
                nouvelleTuile.Style = Resources["StyleTuileAtout"] as Style;

            return nouvelleTuile;
        }

        private StackPanel CreerStackPanel(int pIndexJoueur)
        {
            StackPanel nouveauStackPanel = new StackPanel
            {
                Uid = Guid.NewGuid().ToString(),
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Canvas.SetLeft(nouveauStackPanel, 0);
            Canvas.SetTop(nouveauStackPanel, pIndexJoueur * 100);
            canvasTableJeu.Children.Add(nouveauStackPanel);

            return nouveauStackPanel;
        }

        private UIElement TrouverUIElementParUID(DependencyObject pRootElement, Guid pID)
        {
            foreach (UIElement element in LogicalTreeHelper.GetChildren(pRootElement).OfType<UIElement>())
            {
                if (element.Uid == pID.ToString())
                    return element;

                UIElement resultChildren = TrouverUIElementParUID(element, pID);

                if (resultChildren != null)
                    return resultChildren;
            }

            return null;
        }
    }
}