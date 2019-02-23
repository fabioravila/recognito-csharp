using Recognito.Enchancements;
using Recognito.Features;
using Recognito.Utils;
using System.IO;

namespace Recognito.Vad
{
    public class PreprocessorAndFeatureExtractor
    {
        readonly IFeaturesExtractor<double[]> extractor;
        readonly Normalizer normalizer;
        readonly AutocorrellatedVoiceActivityDetector voiceDetector;
        readonly float sampleRate;

        public PreprocessorAndFeatureExtractor(float sampleRate)
        {
            extractor = new LpcFeaturesExtractor(sampleRate, 20);
            voiceDetector = new AutocorrellatedVoiceActivityDetector();
            normalizer = new Normalizer();
            this.sampleRate = sampleRate;
        }


        public double[] ProcessAndExtract(double[] voiceSample)
        {
            var processed = voiceSample;

            processed = voiceDetector.RemoveSilence(processed, sampleRate);
            normalizer.Normalize(processed, sampleRate);
            processed = extractor.ExtractFeatures(processed);

            return processed;
        }


        public double[] ProcessAndExtract(Stream voiceSample)
        {
            var processed = AudioConverter.ConvertAudioToDoubleArray(voiceSample, sampleRate);

            processed = voiceDetector.RemoveSilence(processed, sampleRate);
            normalizer.Normalize(processed, sampleRate);
            processed = extractor.ExtractFeatures(processed);

            return processed;
        }
    }
}
