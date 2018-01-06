using Recognito.Distances;
using System;
using System.Collections.Generic;
using System.IO;

namespace Recognito.SpeakerFinder
{
    public class AbsoluteEuclideanDistBelowThresholdIsAMatch : ISpeakerFinderAlgorithm
    {
        readonly DistanceCalculator calculator;
        const int frameRate = 16000;
        readonly double distanceThreshold;

        public AbsoluteEuclideanDistBelowThresholdIsAMatch(double distanceThreshold)
        {
            calculator = new EuclideanDistanceCalculator();
            this.distanceThreshold = distanceThreshold;
        }

        public List<Match> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string toBeScreenedForAudioFilesWithSpeakerFolder)
        {
            var result = new List<Match>();
            var speakerPrint = VoicePrint.FromStream(speakerAudioFile, frameRate);

            foreach (var file in Directory.GetFiles(toBeScreenedForAudioFilesWithSpeakerFolder, "*.wav", SearchOption.TopDirectoryOnly))
            {
                using (var fs = new FileStream(file, FileMode.Open))
                {
                    var vp_test = VoicePrint.FromStream(fs, frameRate);

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
