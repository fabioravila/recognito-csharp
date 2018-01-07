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
        readonly PreprocessorAndFeatureExtractor featureExtractor;

        public AbsoluteEuclideanDistBelowThresholdForPtcOfWordsIsAMatch(double distanceThreshold, double wordsPctThreshold)
        {
            calculator = new EuclideanDistanceCalculator();
            voiceDetector = new AutocorrellatedVoiceActivityDetector();

            this.distanceThreshold = distanceThreshold;
            this.wordsPctThreshold = wordsPctThreshold;

            featureExtractor = new PreprocessorAndFeatureExtractor(sampleRate);
        }

        public List<Match> FindAudioFilesContainingSpeaker(Stream speakerAudioFile, string toBeScreenedForAudioFilesWithSpeakerFolder)
        {
            var result = new List<Match>();


            var speakerVoicePrint = VoicePrint.FromFeatures(featureExtractor.ProcessAndExtract(speakerAudioFile));

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
                        var fVoicePrint = VoicePrint.FromFeatures(featureExtractor.ProcessAndExtract(fs));
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

