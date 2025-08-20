using System;
using UnityEngine;

namespace HexGrid
{
    public class HexConvolution
    {
        float[] weights;
        int radius;

        public static HexConvolution GaussianSmoothing(int radius, float standardDeviation = 1.57079632679f)
        {
            if(radius < 0) radius = 0;

            float sum = 0;
            for (int i = 0; i <= radius; i++)
                sum += RingCellsCount(i) * Profile(i);

            HexConvolution convolution = new();
            convolution.radius = radius;
            convolution.weights = new float[radius+1];
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


        /// <param name="add">A delegate that adds 2 "TOut" together</param>
        /// <param name="scale">A delegate that multiply a "TOut" by a float weight</param>
        /// <param name="get">A delegate Get(HexPosition center, TIn othercell) that returns the value that will be used>
        /// <returns></returns>
        public HexGrid<TOut> ApplyTo<TIn, TOut>(
            HexGrid<TIn> grid,
            Func<TOut, TOut, TOut> add,
            Func<TOut, float, TOut> scale,
            Func<HexPosition, TIn, TOut> get
        )
        {
            HexGrid<TOut> grid2 = new();

            foreach (HexPosition pos in grid)
            {
                TOut tot = default!;
                float totWeight = 0f;
                bool hasValue = false;

                foreach (HexPosition pos2 in pos.GetNeighbors(radius))
                {
                    if (!grid.TryGet(pos2, out TIn value)) continue;

                    int d = pos2.DistanceTo(pos);
                    float w = weights[Math.Clamp(d, 0, weights.Length - 1)];
                    totWeight += w;

                    TOut v2 = get(pos, value);
                    tot = hasValue ? add(tot, scale(v2, w)) : scale(v2, w);
                    hasValue = true;
                }

                if (hasValue)
                    grid2[pos] = scale(tot, 1f / totWeight);
            }

            return grid2;
        }


    }
}
