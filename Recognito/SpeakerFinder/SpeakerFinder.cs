using Recognito.Distances;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognito.SpeakerFinder
{
    public class SpeakerFinder
    {
        readonly Recognito<string> recognito;
        readonly DistanceCalculator calculator;

        const string FAVORITE_SPEAKER = "favoriteSpeaker";
        double threshold;

        public SpeakerFinder(double threshold)
        {
            recognito = new Recognito<string>();
            calculator = new EuclideanDistanceCalculator();
            this.threshold = threshold;
        }

        List<string> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string filesSearchSpeaker)
        {
            if (!Directory.Exists(filesSearchSpeaker))
                throw new ArgumentNullException(nameof(filesSearchSpeaker), "Directory not find");

            var result = new List<string>();

            var speakerPrint = recognito.CreateVoicePrint(FAVORITE_SPEAKER, speakerAudioFile);

            foreach (var file in Directory.GetFiles(filesSearchSpeaker, "*.wav", SearchOption.TopDirectoryOnly))
            {
                using (var fs = new FileStream(file, FileMode.Open))
                {
                    var vp_test = recognito.CreateVoicePrint(Guid.NewGuid().ToString(), fs);

                    if (vp_test.GetDistance(calculator, speakerPrint) > threshold)
                    {
                        result.Add(file);
                    }
                }
            }

            return result;
        }
    }
}
