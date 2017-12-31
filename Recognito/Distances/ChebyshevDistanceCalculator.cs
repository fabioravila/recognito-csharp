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

namespace Recognito.Distances
{
    class ChebyshevDistanceCalculator : DistanceCalculator
    {
        public override double GetDistance(double[] features1, double[] features2)
        {
            double distance = PositiveInfinityIfEitherOrBothAreNull(features1, features2);

            if (distance < 0)
            {
                if (features1.Length != features2.Length)
                    throw new ArgumentException($"Both features should have the same length. Received lengths of [{ features1.Length }] and [{features2.Length}]");

                distance = 0.0;

                for (int i = 0; i < features1.Length; i++)
                {
                    var currentDistance = Math.Abs(features1[i] - features2[i]);

                    distance = (currentDistance > distance) ? currentDistance : distance;
                }
            }

            return distance;
        }
    }
}
