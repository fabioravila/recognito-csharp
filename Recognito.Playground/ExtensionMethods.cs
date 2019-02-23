using NAudio.Wave;
using System.Threading;

namespace Recognito.Playground
{
    public static class ExtensionMethods
    {

        public static void PlayAndWait(this WaveOutEvent waveOut)
        {
            if (waveOut == null)
                return;

            waveOut.Play();

            while (waveOut.PlaybackState != PlaybackState.Stopped) { Thread.Sleep(50); }
        }

    }
}
