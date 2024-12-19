using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberMan.Services
{
    internal class CachedSound
    {
        public float[] AudioData { get; private set; }
        public WaveFormat WaveFormat { get; private set; }

        public CachedSound(string relativeFilePath)
        {
            // Hämta den aktuella körkatalogen (där .exe körs)
            string exePath = AppDomain.CurrentDomain.BaseDirectory;

                                                                            // Detta ger en sökväg till Bomberman mappen 
            string projectRootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(exePath).FullName).FullName).FullName).FullName;

            string fullPath = Path.Combine(projectRootPath, relativeFilePath);

            // Kontrollera om filen existerar
            if (!File.Exists(fullPath))
            {
                //C:\Users\johan\Source\Repos\systemvetenskap\ht24-sup24_g3\BomberMan\Assets\Audio\Effects\explosion_01.wav
                throw new FileNotFoundException("Ljudfilen hittades inte", fullPath);
            }

            // Läs ljudfilen med hjälp av NAudio
            using (var audioFileReader = new AudioFileReader(fullPath))
            {
                // TODO: could add resampling in here if required
                WaveFormat = audioFileReader.WaveFormat;
                var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
                var readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
                int samplesRead;
                while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    wholeFile.AddRange(readBuffer.Take(samplesRead));
                }
                AudioData = wholeFile.ToArray();
            }
        }
    }
}
