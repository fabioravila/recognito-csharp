using Recognito.Distances;
using Recognito.Vad;
using System.Collections.Generic;
using System.IO;

namespace Recognito.SpeakerFinder
{
    public class AbsoluteEuclideanDistBelowThresholdIsAMatch : ISpeakerFinderAlgorithm
    {
        readonly DistanceCalculator calculator;
        const int frameRate = 16000;
        readonly double distanceThreshold;
        readonly PreprocessorAndFeatureExtractor featureExtractor;

        public AbsoluteEuclideanDistBelowThresholdIsAMatch(double distanceThreshold)
        {
            calculator = new EuclideanDistanceCalculator();
            this.distanceThreshold = distanceThreshold;
            featureExtractor = new PreprocessorAndFeatureExtractor(frameRate);
        }

        public List<Match> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string toBeScreenedForAudioFilesWithSpeakerFolder)
        {
            var result = new List<Match>();

            var speakerPrint = VoicePrint.FromFeatures(featureExtractor.ProcessAndExtract(speakerAudioFile));

            foreach (var file in Directory.GetFiles(toBeScreenedForAudioFilesWithSpeakerFolder, "*.wav", SearchOption.TopDirectoryOnly))
            {
                using (var fs = new FileStream(file, FileMode.Open))
                {
                    var vp_test = VoicePrint.FromFeatures(featureExtractor.ProcessAndExtract(fs));
                    var distance = vp_test.GetDistance(calculator, speakerPrint);

                    if (distance > distanceThreshold)
                    {
                        result.Add(new Match(file, distance));
                    }
                }
            }

            return result;
        }
    }
}
