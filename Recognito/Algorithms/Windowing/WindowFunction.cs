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

namespace Recognito.Algorithms.Windowing
{
    public abstract class WindowFunction
    {
        protected static readonly double TWO_PI = 2 * Math.PI;

        readonly int windowSize;
        readonly double[] factors;


        /**
      * Constructor of a WindowFunction
      * <p>
      * Please note this constructor precomputes all coefficiencies for the given window size
      * </p>
      * @param windowSize the window size
      */
        public WindowFunction(int windowSize)
        {
            this.windowSize = windowSize;
            factors = GetPrecomputedFactors(windowSize);
        }


        /**
         * Applies window function to an array of doubles
         * @param window array of doubles to apply windowing to
         */
        public void ApplyFunction(double[] window)
        {
            if (window.Length == windowSize)
            {
                for (int i = 0; i < window.Length; i++)
                {
                    window[i] *= factors[i];
                }
            }
            else
            {
                throw new ArgumentException($"Incompatible window size for this WindowFunction instance : expected {windowSize}, received {window.Length}");
            }
        }

        /**
         * Precomputes factors to be applied for this function, called from constructor<br/>
         * Implementing classes are strongly advised to cache the results for subsequent instances
         * @param windowSize the window size
         * @return the precomputed factors
         */
        protected abstract double[] GetPrecomputedFactors(int windowSize);
    }
}
