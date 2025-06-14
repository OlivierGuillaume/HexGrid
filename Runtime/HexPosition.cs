using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HexGrid
{
    /// <summary>
    /// Pointy top hexagon grid position
    /// </summary>
    [System.Serializable]
    public readonly struct HexPosition : IComparable<HexPosition>
    {
        readonly public int q;
        readonly public int r;
        public readonly int S => -q - r;

        public readonly Vector2Int OffsetCoordinates => new(q + (r - (r & 1)) / 2, r);
        public readonly Vector3 WorldPosition => new(HORIZ * (q + 0.5f * r), 0f, - VERT * r);

        internal const float SIZE = 1f;//Lenght of the hexagons sides
        internal const float SR3 = 1.7320508075688772935274463415059f;//square root of 3
        internal const float HORIZ = SIZE * SR3;//Horizontal distance between to hexes of the same row
        internal const float VERT = SIZE * 1.5f;//Vertical distance between two ajacent rows

        #region constructors

        public HexPosition(int q, int r, bool offsetCoordinates = false)
        {
            this.q = offsetCoordinates ? q - (r - (r & 1)) / 2 : q;
            this.r = r;
        }

        public HexPosition(Vector2Int qr, bool offsetCoordinates = false) : this(qr.x, qr.y, offsetCoordinates)
        {
        }

        public static HexPosition GetNearestTo(Vector3 worldPosition)
        {
            float q = (SR3 * worldPosition.x + worldPosition.z) / 3f;
            float r = -2f / 3f * worldPosition.z;
            float s = -q - r;

            int Q = Mathf.RoundToInt(q);
            int R = Mathf.RoundToInt(r);
            int S = Mathf.RoundToInt(s);

            float dq = Mathf.Abs(q - Q);
            float dr = Mathf.Abs(r - R);
            float ds = Mathf.Abs(s - S);

            if (dq > dr && dq > ds)
            {
                Q = -R - S;
            }
            else if (dr > ds)
            {
                R = -Q - S;
            }

            return new(Q, R);
        }
        #endregion

        #region Overrides
        public static HexPosition operator +(HexPosition a, HexPosition b) => new(a.q + b.q, a.r + b.r);
        public static HexPosition operator -(HexPosition a, HexPosition b) => new(a.q - b.q, a.r - b.r);
        public static HexPosition operator -(HexPosition a) => new(-a.q, -a.r);
        public static HexPosition operator *(HexPosition a, int i) => new(i * a.q, i * a.r);
        public static HexPosition operator *(int i, HexPosition a) => new(i * a.q, i * a.r);
        public static bool operator ==(HexPosition a, HexPosition b) => a.q == b.q && a.r == b.r;
        public static bool operator !=(HexPosition a, HexPosition b) => a.q != b.q || a.r != b.r;

        public override int GetHashCode()
        {
            return (r << 16) + q;
        }

        public int CompareTo(HexPosition other)
        {
            return GetHashCode() - other.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is HexPosition position &&
                   q == position.q &&
                   r == position.r;
        }

        public override string ToString()
        {
            return $"({q},{r},{S})";
        }
        #endregion

        #region neighbors and distances

        /// <summary>
        /// Number of hexagons in a distance less than range (including the origin hex)
        /// </summary>
        private static int NumberOfHexesInRange(int range)
        {
            int size = range + 1;

            if (size <= 0) return 0;

            int n = 1;
            for (int i = 1; i < size; i++)
            {
                n += 6 * i;
            }
            return n;
        }
        private static HexPosition[] GetNeighborsOffsets(int maxDistance)
        {
            HexPosition[] offsets = new HexPosition[NumberOfHexesInRange(maxDistance) - 1];
            int i = 0;
            int lowerR, higherR;

            if (maxDistance == 1)
            {
                //Set it manually so it is clockwise (important for some other scripts)
                offsets[0] = new HexPosition(1, -1);
                offsets[1] = new HexPosition(1, 0);
                offsets[2] = new HexPosition(0, 1);
                offsets[3] = new HexPosition(-1, 1);
                offsets[4] = new HexPosition(-1, 0);
                offsets[5] = new HexPosition(0, -1);
                return offsets;
            }

            for (int q = -maxDistance; q <= maxDistance; q++)
            {
                lowerR = Mathf.Max(-maxDistance, -maxDistance - q);
                higherR = Mathf.Min(maxDistance, maxDistance - q);
                for (int r = lowerR; r <= higherR; r++)
                {
                    if (q == 0 && r == 0) continue;
                    offsets[i++] = new HexPosition(q, r);
                }
            }
            return offsets;
        }

        private static readonly Dictionary<int, HexPosition[]> _neighborsOffsetsCache = new();

        public readonly HexPosition[] GetAdjacentHexes() => GetNeighbors(1); 
        public readonly HexPosition[] GetNeighbors(int maxDistance)
        {
            if (maxDistance <= 0) return new HexPosition[0];

            if (!_neighborsOffsetsCache.ContainsKey(maxDistance))
                _neighborsOffsetsCache[maxDistance] = GetNeighborsOffsets(maxDistance);

            HexPosition[] offsets = _neighborsOffsetsCache[maxDistance];

            HexPosition[] neighbors = new HexPosition[offsets.Length];

            for (int i = 0; i < offsets.Length; i++)
            {
                neighbors[i] = this + offsets[i];
            }

            return neighbors;
        }

        public readonly HexVertex[] GetAjactentVertices()
        {
            HexVertex[] adj = new HexVertex[6];

            for (int i = 0; i < 6; i++) adj[i] = new(this, i);

            return adj;
        }

        public readonly HexEdge[] GetAdjacentEdges()
        {
            HexPosition[] adjHexes = GetAdjacentHexes();
            HexEdge[] adj = new HexEdge[adjHexes.Length];

            for (int i = 0; i < adjHexes.Length; i++) adj[i] = new(this, adjHexes[i]);

            return adj;
        }

        public readonly int DistanceTo(HexPosition other)
        {
            return (other - this).Magnitude;
        }

        public readonly int Magnitude => (Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(S)) / 2;
        #endregion

        #region Rotation
        /// <summary>
        /// Rotate a position around a center by increment of 60�
        /// </summary>
        /// <param name="position"></param>
        /// <param name="center"></param>
        /// <param name="angleIncrement">Rotate by 60� clockwise multiplied by this number, negative values for counter-clockwise rotation</param>
        /// <returns></returns>
        public HexPosition Rotate60Around(HexPosition center = default, int angleIncrement = 1)
        {
            int n = angleIncrement % 6;
            n = (n + 6) % 6;

            HexPosition p = this - center;

            switch (n)
            {
                case 0: return this;
                case 1: return center + new HexPosition(-p.r, -p.S);
                case 2: return center + new HexPosition(p.S, p.q);
                case 3: return center - p;
                case 4: return center + new HexPosition(p.r, p.S);
                case 5: return center + new HexPosition(-p.S, -p.q);
                default: return this;
            }
        }
        #endregion
    }
}
