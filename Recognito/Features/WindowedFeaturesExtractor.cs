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

namespace Recognito.Features
{
    public abstract class WindowedFeaturesExtractor<T> : IFeaturesExtractor<T>
    {
        private static readonly int DEFAULT_TARGET_WINDOW_LENGTH_IN_MILLIS = 24;
        private static readonly float MIN_SAMPLE_RATE = 8000.0F;

        protected readonly int windowSize;
        protected readonly float sampleRate;

        /**
   * Base constructor required by this abstract class
   * @param sampleRate the sample rate of the voice samples, minimum 8000.0
   */
        public WindowedFeaturesExtractor(float sampleRate)
        {
            if (sampleRate < MIN_SAMPLE_RATE)
            {
                throw new ArgumentException("Sample rate should be at least 8000 Hz");
            }
            this.sampleRate = sampleRate;
            this.windowSize = GetWindowSize(sampleRate);
        }

        /* (non-Javadoc)
         * @see com.recognito.processing.features.FeaturesExtractor#extractFeatures(double[])
         */
        public abstract T ExtractFeatures(double[] voiceSample);

        /**
         * Called by the constructor of this class.
         * This implementation delegates to {@link #getClosestPowerOfTwoWindowSize(float, int)}
         * with default targetSizeInMillis value
         * <p>
         * Implementing classes may wish to override this method by delegating with another target value in millis
         * or implement another logic alltogether
         * </p>
         * @param sampleRate the sample rate in Hz (times per second), minimum 8000.0
         * @return the window size
         */
        protected int GetWindowSize(float sampleRate)
        {
            return GetClosestPowerOfTwoWindowSize(sampleRate, DEFAULT_TARGET_WINDOW_LENGTH_IN_MILLIS);
        }

        /**
         * Computes the window size that is both the closest to targetSizeInMillis and a power of 2
         * <p>
         * Note : window size using a power of 2 is required e.g. when using discrete FFT algorithm
         * </p>
         * @param sampleRate the sample rate in Hz (times per second), minimum 8000.0
         * @param targetSizeInMillis the target size in millis
         * @return the window size
         */
        protected int GetClosestPowerOfTwoWindowSize(float sampleRate, int targetSizeInMillis)
        {
            bool done = false;
            int pow = 8; // 8 bytes == 1ms at 8000 Hz
            float previousMillis = 0.0f;

            while (!done)
            {
                float millis = 1000 / sampleRate * pow;
                if (millis < targetSizeInMillis)
                {
                    previousMillis = millis;
                    pow *= 2;
                }
                else
                {
                    // closest value to target wins
                    if (Math.Abs(targetSizeInMillis - millis) > targetSizeInMillis - previousMillis)
                    {
                        pow /= 2; // previousMillis was closer
                    }
                    done = true;
                }
            }
            return pow;
        }
    }
}
