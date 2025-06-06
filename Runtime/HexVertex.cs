using System;
using UnityEngine;

namespace HexGrid
{
    public readonly struct HexVertex : IComparable<HexVertex>
    {
        readonly HexPosition hex;
        readonly bool isY;//each vertex has 3 sides, forming either a Y or a vertically flipped Y


        static readonly Vector3 OFFSET_NOT_Y = new Vector3(0f, 0f, HexPosition.SIZE);
        static readonly Vector3 OFFSET_Y = new Vector3(0.5f * HexPosition.HORIZ, 0f, 0.5f * HexPosition.SIZE);

        /// <param name="direction">the direction of the vertex relative to the hex. 0 is north, then rotate clockwise</param>
        public HexVertex(HexPosition hex, int direction)
        {
            direction = (direction % 6 + 6) % 6;
            this.hex = hex;
            isY = direction % 2 == 1;
            switch (direction)
            {
                case 2: 
                    this.hex += new HexPosition(0,1);  
                    break;
                case 3:
                    this.hex += new HexPosition(-1, 1);
                    break;
                case 4:
                    this.hex += new HexPosition(-1, 1);
                    break;
                case 5:
                    this.hex += new HexPosition(-1, 0);
                    break;
            }
        }


        public static HexVertex GetNearestTo(Vector3 worldPosition)
        {
            float q = (HexPosition.SR3 * worldPosition.x + worldPosition.z) / 3f;
            float r = -2f / 3f * worldPosition.z;
            float s = -q - r;

            int Q = Mathf.FloorToInt(q);
            int R = Mathf.CeilToInt(r);
            int S = Mathf.FloorToInt(s);

            return new HexVertex(new(Q, R), Q + R + S == 0 ? 0 : 1);
        }

        public Vector3 WorldPosition => hex.WorldPosition + (isY ? OFFSET_Y : OFFSET_NOT_Y);

        public HexPosition[] GetAdjacentHexes()
        {
            if (isY)
            {
                return new HexPosition[] {
                    hex,
                    hex + new HexPosition(1, -1),
                    hex + new HexPosition(1, 0)
                };
            }
            else
            {
                return new HexPosition[] {
                    hex,
                    hex + new HexPosition(0, -1),
                    hex + new HexPosition(1, -1)
                };
            }
        }

        public HexEdge[] GetAdjacentEdges()
        {
            HexPosition A = hex + new HexPosition(1, -1);
            HexPosition B = hex + (isY ? new HexPosition(1, 0) : new HexPosition(0, -1));

            return new HexEdge[] {
                new HexEdge(hex, A),
                new HexEdge(hex, B),
                new HexEdge(A, B),
            };
        }

        public HexVertex[] GetAdjacentVertices()
        {
            if (isY)
            {
                return new HexVertex[] {
                    new HexVertex(hex, 0),
                    new HexVertex(hex, 2),
                    new HexVertex(hex + new HexPosition(1,0), 0),
                };
            }
            else
            {
                return new HexVertex[] {
                    new HexVertex(hex, 1),
                    new HexVertex(hex, 5),
                    new HexVertex(hex + new HexPosition(0,-1), 1),
                };
            }
        }

        public override int GetHashCode()
        {
            return hex.GetHashCode() << 1 + (isY ? 1 : 0);
        }

        public static bool operator ==(HexVertex x, HexVertex y) => x.hex == y.hex && x.isY == y.isY;
        public static bool operator !=(HexVertex x, HexVertex y) => x.hex != y.hex || x.isY != y.isY;

        public override bool Equals(object obj)
        {
            return obj is HexVertex v &&
                   v.hex == hex &&
                   v.isY == isY;
        }

        public int CompareTo(HexVertex other)
        {
            return GetHashCode() - other.GetHashCode();
        }

        public override string ToString()
        {
            return $"{hex} {(isY ? 1 : 0)})";
        }
    }
}
