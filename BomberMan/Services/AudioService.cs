using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BomberMan.Services
{
    public class AudioService
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();

        private MediaPlayer effectPlayer = new MediaPlayer();

        public void PlayClip(string audioName)
        {
            string path = LoadAudio(audioName);  // Helper to get full path of audio file
            mediaPlayer.Open(new Uri(path, UriKind.Absolute));
            mediaPlayer.Volume = 1.0; // Ensure the volume is at max
            mediaPlayer.Play();
        }

        public void PlaySoundEffect(string effectName)
        {
            string path = LoadAudio(effectName);  // Helper to get full path of audio file
            effectPlayer.Open(new Uri(path, UriKind.Absolute));
            effectPlayer.Volume = 1.0;  // Adjust as necessary for sound effects
            effectPlayer.Play();
        }

        public void StopClip()
        {
            mediaPlayer.Stop();
        }

        private string LoadAudio(string audioName)
        {
            return AppDomain.CurrentDomain.BaseDirectory + $"\\Assets\\Audio\\Music\\{audioName}.wav";
        }
    }


}

