using System;
using UnityEngine;

namespace HexGrid
{
    public class HexConvolution
    {
        float[] weights;
        int radius;

        public HexConvolution GaussianSmoothing(int radius, float standardDeviation = 1.57079632679f)
        {
            if(radius < 0) radius = 0;

            float sum = 0;
            for (int i = 0; i <= radius; i++)
                sum += RingCellsCount(i) * Profile(i);

            HexConvolution convolution = new();
            convolution.radius = radius;
            convolution.weights = new float[radius];
            for (int i = 0; i <= radius; i++)
                convolution.weights[i] = Profile(i)/sum;

            return convolution;

            float Profile(int d) => Mathf.Exp(-d * d / (2f * standardDeviation * standardDeviation));
            float RingCellsCount(int d) => d <= 0 ? 1 : 6 * d;
        }

        public HexGrid<float> ApplyTo(HexGrid<float> grid)
        {
            HexGrid<float> grid2 = new();

            foreach(HexPosition pos in grid)
            {
                float tot = 0f;
                float totWeight = 0f;//keeping track of it to normalize in case of missing cell (outside of the grid or holes)

                foreach(HexPosition pos2 in pos.GetNeighbors(radius))
                {
                    if(!grid.TryGet(pos2, out float value)) continue;

                    int d = pos2.DistanceTo(pos);
                    float w = weights[Mathf.Clamp(d, 0, weights.Length - 1)];
                    totWeight += w;

                    tot += w * value;
                }

                grid2[pos] = tot / totWeight;
            }

            return grid2;
        }


        /// <param name="add">A delegate that adds 2 "T" together</param>
        /// <param name="scale">A delegate that multiply a "T" by a float weight</param>
        /// <returns></returns>
        public HexGrid<T> ApplyTo<T>(
            HexGrid<T> grid,
            Func<T, T, T> add,
            Func<T, float, T> scale
        )
        {
            HexGrid<T> grid2 = new();

            foreach (HexPosition pos in grid)
            {
                T tot = default!;
                float totWeight = 0f;
                bool hasValue = false;

                foreach (HexPosition pos2 in pos.GetNeighbors(radius))
                {
                    if (!grid.TryGet(pos2, out T value)) continue;

                    int d = pos2.DistanceTo(pos);
                    float w = weights[Math.Clamp(d, 0, weights.Length - 1)];
                    totWeight += w;

                    tot = hasValue ? add(tot, scale(value, w)) : scale(value, w);
                    hasValue = true;
                }

                if (hasValue)
                    grid2[pos] = scale(tot, 1f / totWeight);
            }

            return grid2;
        }


    }
}
