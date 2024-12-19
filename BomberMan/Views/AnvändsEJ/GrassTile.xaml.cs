using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BomberMan.Views.Tiles
{
    /// <summary>
    /// Interaction logic for GrassTile.xaml
    /// </summary>
    public partial class GrassTile : UserControl
    {
        public SolidColorBrush GrassColor
        {
            get { return (SolidColorBrush)GetValue(GrassColorProperty); }
            set { SetValue(GrassColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OceanColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GrassColorProperty =
            DependencyProperty.Register("GrassColor", typeof(SolidColorBrush), typeof(GrassTile), new PropertyMetadata(Brushes.Green));


        public GrassTile()
        {
            InitializeComponent();
        }
    }
}
