using System.IO;

namespace Recognito.Tests.Helpers
{
    public class SpeakerAudioInfo
    {
        public static SpeakerAudioInfo From_ST_AEDS_File(string fullPath)
        {
            //sample filename f0001_us_f0001_00296.wav or m0002_us_m0002_00282.wav
            var pattern = Path.GetFileName(fullPath);

            if (!pattern.EndsWith(".wav"))
                return null;

            return new SpeakerAudioInfo
            {
                FullFileName = pattern,
                FullPath = fullPath,
                SpeakerGender = pattern.Substring(0, 1).ToUpper()[0],
                SpeakerId = pattern.Substring(0, 5)
            };
        }


        public static SpeakerAudioInfo From_Mini_LibriSpeech_ASR(string fullPath)
        {
            //sample filename 19-198-0000.flac
            var pattern = Path.GetFileName(fullPath);

            if (!pattern.EndsWith(".flac"))
                return null;

            return new SpeakerAudioInfo
            {
                FullFileName = pattern,
                FullPath = fullPath,
                SpeakerGender = 'N',
                SpeakerId = pattern.Substring(0, pattern.IndexOf('-'))
            };
        }

        public char SpeakerGender { get; set; }
        public string SpeakerId { get; set; }
        public string FullPath { get; set; }
        public string FullFileName { get; set; }
    }
}
