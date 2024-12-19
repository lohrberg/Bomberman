using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BomberMan.Services
{
    // Interface för att ladda bilder
    public interface IImageLoaderService
    {
        BitmapImage LoadImage(string relativePath);
    }

    // Metod för att ladda en bild baserat på sökväg
    public class ImageLoaderService : IImageLoaderService
    {
        public BitmapImage LoadImage(string relativePath)
        {
            var uri = new Uri($"pack://application:,,,/BomberMan;component/{relativePath}", UriKind.Absolute);

            // Returnerar en ny BitmapImage från den skapade Uri
            return new BitmapImage(uri);
        }
    }
}
