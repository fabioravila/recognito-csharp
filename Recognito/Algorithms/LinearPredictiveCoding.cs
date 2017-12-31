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

namespace Recognito.Algorithms
{
    public class LinearPredictiveCoding
    {
        private readonly int windowSize;
        private readonly int poles;
        private readonly double[] output;
        private readonly double[] error;
        private readonly double[] k;
        private readonly double[][] matrix;


        public LinearPredictiveCoding(int windowSize, int poles)
        {
            this.windowSize = windowSize;
            this.poles = poles;
            output = new double[poles];
            error = new double[poles];
            k = new double[poles];
            matrix = new double[poles][];
        }


        public double[][] applyLinearPredictiveCoding(double[] window)
        {

            if (windowSize != window.Length)
            {
                throw new ArgumentException($"Given window length was not equal to the one provided in constructor : [{window.Length}] != [{windowSize}]");
            }

            ArrayHelper.Fill(k, 0.0d);
            ArrayHelper.Fill(output, 0.0d);
            ArrayHelper.Fill(error, 0.0d);

            foreach (var d in matrix)
            {
                ArrayHelper.Fill(d, 0.0d);
            }


            DiscreteAutocorrelationAtLagJ dalj = new DiscreteAutocorrelationAtLagJ();
            double[] autocorrelations = new double[poles];
            for (int i = 0; i < poles; i++)
            {
                autocorrelations[i] = dalj.Autocorrelate(window, i);
            }

            error[0] = autocorrelations[0];

            for (int m = 1; m < poles; m++)
            {
                double tmp = autocorrelations[m];
                for (int i = 1; i < m; i++)
                {
                    tmp -= matrix[m - 1][i] * autocorrelations[m - i];
                }
                k[m] = tmp / error[m - 1];

                for (int i = 0; i < m; i++)
                {
                    matrix[m][i] = matrix[m - 1][i] - k[m] * matrix[m - 1][m - i];
                }
                matrix[m][m] = k[m];
                error[m] = (1 - (k[m] * k[m])) * error[m - 1];
            }

            for (int i = 0; i < poles; i++)
            {
                if (Double.IsNaN(matrix[poles - 1][i]))
                {
                    output[i] = 0.0;
                }
                else
                {
                    output[i] = matrix[poles - 1][i];
                }
            }

            return new double[][] { output, error };
        }
    }
}
