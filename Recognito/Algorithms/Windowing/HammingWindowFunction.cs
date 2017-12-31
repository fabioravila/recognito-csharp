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

namespace Recognito.Algorithms.Windowing
{
    public class HammingWindowFunction : WindowFunction
    {
        private static readonly object _lock = new object();

        private static readonly Dictionary<int, double[]> factorsByWindowSize = new Dictionary<int, double[]>();


        public HammingWindowFunction(int windowSize) : base(windowSize) { }

        protected override double[] GetPrecomputedFactors(int windowSize)
        {
            // precompute factors for given window, avoid re-calculating for several instances
            lock (_lock)
            {
                if (factorsByWindowSize.ContainsKey(windowSize))
                    return factorsByWindowSize[windowSize];


                var factors = new double[windowSize];
                int sizeMinusOne = windowSize - 1;
                for (int i = 0; i < windowSize; i++)
                {
                    factors[i] = 0.54d - (0.46d * Math.Cos((TWO_PI * i) / sizeMinusOne));
                }
                factorsByWindowSize.Add(windowSize, factors);
                return factors;
            }
        }
    }
}
