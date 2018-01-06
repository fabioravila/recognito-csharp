/*
 * (C) Copyright 2014 Amaury Crickx
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognito.Vad
{
    public class AutocorrellatedVoiceActivityDetector
    {
        static readonly int WINDOW_MILLIS = 1;
        static readonly int FADE_MILLIS = 2;
        static readonly int MIN_SILENCE_MILLIS = 4;
        static readonly int MIN_VOICE_MILLIS = 200;

        double threshold = 0.0001d;

        double[] fadeInFactors;
        double[] fadeOutFactors;


        /**
    * Returns the noise threshold used to determine if a given section is silence or not
    * @return the threshold
    */
        public double AutocorrellationThreshold
        {
            get { return threshold; }
            set { threshold = value; }
        }


        /**
         * Removes silence out of the given voice sample
         * @param voiceSample the voice sample
         * @param sampleRate the sample rate
         * @return a new voice sample with silence removed
         */

        public double[] RemoveSilence(double[] voiceSample, float sampleRate)
        {
            int oneMilliInSamples = (int)sampleRate / 1000;

            int length = voiceSample.Length;
            int minSilenceLength = MIN_SILENCE_MILLIS * oneMilliInSamples;
            int minActivityLength = GetMinimumVoiceActivityLength(sampleRate);
            bool[] result = new bool[length];

            if (length < minActivityLength)
            {
                return voiceSample;
            }

            int windowSize = WINDOW_MILLIS * oneMilliInSamples;
            double[] correllation = new double[windowSize];
            double[] window = new double[windowSize];


            for (int position = 0; position + windowSize < length; position += windowSize)
            {
                Array.Copy(voiceSample, position, window, 0, windowSize);
                double mean = BruteForceAutocorrelation(window, correllation);
                ArrayHelper.Fill(result, position, position + windowSize, mean > threshold);
            }


            MergeSmallSilentAreas(result, minSilenceLength);

            int silenceCounter = MergeSmallActiveAreas(result, minActivityLength);

            //        System.out.println((int)((double)silenceCounter / result.Length * 100.0d) + "% removed");

            if (silenceCounter > 0)
            {

                int fadeLength = FADE_MILLIS * oneMilliInSamples;
                InitFadeFactors(fadeLength);
                double[] shortenedVoiceSample = new double[voiceSample.Length - silenceCounter];
                int copyCounter = 0;
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i])
                    {
                        // detect lenght of active frame
                        int startIndex = i;
                        int counter = 0;
                        while (i < result.Length && result[i++])
                        {
                            counter++;
                        }
                        int endIndex = startIndex + counter;

                        ApplyFadeInFadeOut(voiceSample, fadeLength, startIndex, endIndex);
                        Array.Copy(voiceSample, startIndex, shortenedVoiceSample, copyCounter, counter);
                        copyCounter += counter;
                    }
                }
                return shortenedVoiceSample;

            }
            else
            {
                return voiceSample;
            }
        }


        public double[][] SplitBySilence(double[] voiceSample, float sampleRate)
        {
            int oneMilliInSamples = (int)sampleRate / 1000;

            int length = voiceSample.Length;
            int minSilenceLength = MIN_SILENCE_MILLIS * oneMilliInSamples;
            int minActivityLength = GetMinimumVoiceActivityLength(sampleRate);
            bool[] result = new bool[length];

            if (length < minActivityLength)
            {
                return new double[][] { voiceSample };
            }

            int windowSize = WINDOW_MILLIS * oneMilliInSamples;
            double[] correlation = new double[windowSize];
            double[] window = new double[windowSize];

            for (int position = 0; position + windowSize < length; position += windowSize)
            {
                Array.Copy(voiceSample, position, window, 0, windowSize);
                double mean = BruteForceAutocorrelation(window, correlation);
                ArrayHelper.Fill(result, position, position + windowSize, mean > threshold);
            }

            MergeSmallSilentAreas(result, minSilenceLength);

            int silenceCounter = MergeSmallActiveAreas(result, minActivityLength);

            if (silenceCounter > 0)
            {
                List<double[]> splitResult = new List<double[]>();
                for (int i = 0; i < result.Length; i++)
                {
                    if (result[i])
                    {
                        // detect lenght of active frame
                        int startIndex = i;
                        int counter = 0;
                        while (i < result.Length && result[i++])
                        {
                            counter++;
                        }

                        double[] newFragment = new double[counter];
                        Array.Copy(voiceSample, startIndex, newFragment, 0, counter);
                        splitResult.Add(newFragment);
                    }
                }
                return splitResult.ToArray();
            }
            else
            {
                return new double[][] { voiceSample };
            }
        }


        /**
         * Gets the minimum voice activity length that will be considered by the remove silence method
         * @param sampleRate the sample rate
         * @return the length
         */
        public int GetMinimumVoiceActivityLength(float sampleRate)
        {
            return MIN_VOICE_MILLIS * (int)sampleRate / 1000;
        }

        /**
         * Applies a linear fade in / out to the given portion of audio (removes unwanted cracks)
         * @param voiceSample the voice sample
         * @param fadeLength the fade length
         * @param startIndex fade in start point
         * @param endIndex fade out end point
         */
        private void ApplyFadeInFadeOut(double[] voiceSample, int fadeLength, int startIndex, int endIndex)
        {
            int fadeOutStart = endIndex - fadeLength;
            for (int j = 0; j < fadeLength; j++)
            {
                voiceSample[startIndex + j] *= fadeInFactors[j];
                voiceSample[fadeOutStart + j] *= fadeOutFactors[j];
            }
        }

        /**
         * Merges small active areas
         * @param result the voice activity result
         * @param minActivityLength the minimum length to apply
         * @return a count of silent elements
         */
        private int MergeSmallActiveAreas(bool[] result, int minActivityLength)
        {
            bool active;
            int increment = 0;
            int silenceCounter = 0;
            for (int i = 0; i < result.Length; i += increment)
            {
                active = result[i];
                increment = 1;
                while ((i + increment < result.Length) && result[i + increment] == active)
                {
                    increment++;
                }
                if (active && increment < minActivityLength)
                {
                    // convert short activity to opposite
                    ArrayHelper.Fill(result, i, i + increment, !active);
                    silenceCounter += increment;
                }
                if (!active)
                {
                    silenceCounter += increment;
                }
            }
            return silenceCounter;
        }

        /**
         * Merges small silent areas
         * @param result the voice activity result
         * @param minSilenceLength the minimum silence length to apply
         */
        private void MergeSmallSilentAreas(bool[] result, int minSilenceLength)
        {
            bool active;
            int increment = 0;
            for (int i = 0; i < result.Length; i += increment)
            {
                active = result[i];
                increment = 1;
                while ((i + increment < result.Length) && result[i + increment] == active)
                {
                    increment++;
                }


                if (!active && increment < minSilenceLength)
                {
                    //ugly fix, i know =], but work
                    var end_idx = Math.Min(i + increment, result.Length - 1);
                    // convert short silence to opposite
                    ArrayHelper.Fill(result, i, end_idx, !active);
                }
            }
        }

        /**
         * Initialize the fade in/ fade out factors properties
         * @param fadeLength
         */
        private void InitFadeFactors(int fadeLength)
        {
            fadeInFactors = new double[fadeLength];
            fadeOutFactors = new double[fadeLength];
            for (int i = 0; i < fadeLength; i++)
            {
                fadeInFactors[i] = (1.0d / fadeLength) * i;
            }
            for (int i = 0; i < fadeLength; i++)
            {
                fadeOutFactors[i] = 1.0d - fadeInFactors[i];
            }
        }

        /**
         * Applies autocorrelation in O² operations. Keep arrays very short !
         * @param voiceSample the voice sample buffer
         * @param correllation the correlation buffer
         * @return the mean correlation value
         */
        private double BruteForceAutocorrelation(double[] voiceSample, double[] correllation)
        {
            ArrayHelper.Fill(correllation, 0);
            int n = voiceSample.Length;
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    correllation[j] += voiceSample[i] * voiceSample[(n + i - j) % n];
                }
            }
            double mean = 0.0d;
            for (int i = 0; i < voiceSample.Length; i++)
            {
                mean += correllation[i];
            }
            return mean / correllation.Length;
        }
    }
}
