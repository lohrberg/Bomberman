using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BomberMan.Services
{
    public class TileValueToColorConverter : IValueConverter
    {
        private readonly IImageLoaderService _imageLoader;

        // Parameterlös konstruktor som krävs för att fungera i XAML
        public TileValueToColorConverter()
        {
            _imageLoader = new ImageLoaderService(); // Använd standardinstans av ImageLoaderService
        }

        // Konstruktor för att injicera ImageLoaderService (för framtida användning utanför XAML)
        public TileValueToColorConverter(IImageLoaderService imageLoader)
        {
            _imageLoader = imageLoader;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enums.Enums.TileMaterial tileMaterial)
            {
                string imageName = tileMaterial switch
                {
                    Enums.Enums.TileMaterial.Grass => "Assets/Images/Tiles/Ground_Tile_01_B.png",
                    Enums.Enums.TileMaterial.Stone => "Assets/Images/Tiles/ground5.png",
                    Enums.Enums.TileMaterial.Dirt => "Assets/Images/Tiles/LightRock_01.png",
                    Enums.Enums.TileMaterial.Fire => "Assets/Images/Tiles/fire.png",
                    _ => "Assets/Images/Tiles/default.png"
                };

                // Använd ImageLoaderService för att ladda bilden
                return _imageLoader.LoadImage(imageName);


            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


