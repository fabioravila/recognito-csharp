using Recognito.Distances;
using Recognito.Utils;
using Recognito.Vad;
using System;
using System.Collections.Generic;
using System.IO;

namespace Recognito.SpeakerFinder
{
    public class AbsoluteEuclideanDistBelowThresholdForPtcOfWordsIsAMatch : ISpeakerFinderAlgorithm
    {
        readonly DistanceCalculator calculator;
        readonly AutocorrellatedVoiceActivityDetector voiceDetector;
        const int sampleRate = 16000;
        readonly double distanceThreshold;
        readonly double wordsPctThreshold;

        public AbsoluteEuclideanDistBelowThresholdForPtcOfWordsIsAMatch(double distanceThreshold, double wordsPctThreshold)
        {
            calculator = new EuclideanDistanceCalculator();
            this.distanceThreshold = distanceThreshold;
            this.wordsPctThreshold = wordsPctThreshold;
            voiceDetector = new AutocorrellatedVoiceActivityDetector();
        }

        public List<Match> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string toBeScreenedForAudioFilesWithSpeakerFolder)
        {
            // We do not care about the learning material - no usage of Universal Model

            var result = new List<Match>();
            var speakerVoicePrint = VoicePrint.FromStream(speakerAudioFile, sampleRate);

            foreach (var file in Directory.GetFiles(toBeScreenedForAudioFilesWithSpeakerFolder, "*.wav", SearchOption.TopDirectoryOnly))
            {
                using (var fs = new FileStream(file, FileMode.Open))
                {
                    double[][] words = voiceDetector.SplitBySilence(AudioConverter.ConvertAudioToDoubleArray(fs, sampleRate), sampleRate);

                    int wordsWithinThreshold = 0;
                    for (int i = 0; i < words.Length; i++)
                    {
                        var wordVoicePrint = VoicePrint.FromFeatures(words[i]);

                        double wordDistance = wordVoicePrint.GetDistance(calculator, speakerVoicePrint);
                        if (wordDistance < distanceThreshold)
                        {
                            wordsWithinThreshold++;
                        }
                    }

                    if (words.Length > 0 && (100.0 * ((double)wordsWithinThreshold / words.Length)) > wordsPctThreshold)
                    {
                        var fVoicePrint = VoicePrint.FromStream(fs, sampleRate);
                        double fDistance = fVoicePrint.GetDistance(calculator, speakerVoicePrint);
                        if (fDistance < distanceThreshold)
                        {
                            result.Add(new Match(file, fDistance));
                        }
                    }
                }
            }


            return result;
        }
    }
}

