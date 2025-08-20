using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.ObjectModel;

namespace HexGrid
{
    public readonly struct HexEdge : IComparable<HexEdge>
    {
        readonly ReadOnlyCollection<HexPosition> EdgeAdjacentHexes;     

        readonly private int hash;

        /// <summary>
        /// Clockwise directions
        /// </summary>
        static readonly List<HexPosition> Directions = new HexPosition(0,0).GetAdjacentHexes().ToList();

        public HexEdge(HexPosition[] hexes) : this(hexes[0], hexes[1])
        {
            if (hexes.Length != 2)
            {
                throw new ArgumentException("Incorrect hexes number used to define a side (must be 2).");
            }
        }

        public HexEdge(HexPosition hex0, HexPosition hex1)
        {
            HexPosition direction = hex1 - hex0;

            if (direction.Magnitude != 1)
            {
                throw new ArgumentException("Hexes must be adjacent to define a side.");
            }

            int directionIndex = Directions.IndexOf(direction);

            if (directionIndex < 3)
            {
                EdgeAdjacentHexes = new(new HexPosition[]{ hex0, hex1 });
            }
            else //ensures that Hex0 and Hex1 are always in the same order
            {
                EdgeAdjacentHexes = new(new HexPosition[] { hex1, hex0 });
                directionIndex = Directions.IndexOf(-direction);
            }

            hash = (EdgeAdjacentHexes[0].GetHashCode() << 2) + directionIndex;

        }

        public HexEdge(HexVertex vertex0, HexVertex vertex1) : this(GetOrthogonalHexes(vertex0, vertex1))
        {
        }

        private static HexPosition[] GetOrthogonalHexes(HexVertex vertex0, HexVertex vertex1)
        {
            var hexes = vertex0.GetAdjacentHexes().ToHashSet();
            hexes.IntersectWith(vertex1.GetAdjacentHexes().ToHashSet());

            if (hexes.Count != 2)
            {
                throw new ArgumentException("Vertices must be adjacent to define a side.");
            }

            return hexes.ToArray();
        }


        /// <summary>
        /// Center of the side in world position
        /// </summary>
        public Vector3 Center => (EdgeAdjacentHexes[0].WorldPosition + EdgeAdjacentHexes[1].WorldPosition) * 0.5f;

        public HexPosition[] GetEdgeAdjacentHexes() => EdgeAdjacentHexes.ToArray();
        public HexPosition[] GetVertexAdjacentHexes(){
            return new HexPosition[]{
                EdgeAdjacentHexes[1].Rotate60Around(EdgeAdjacentHexes[0]),
                EdgeAdjacentHexes[1].Rotate60Around(EdgeAdjacentHexes[0], -1),
            };
        }

        public HexEdge[] GetAdjacentEdges()
        {
            HexEdge[] adj = new HexEdge[4];

            int i = 0;

            foreach(var hex0 in GetVertexAdjacentHexes())
            {
                foreach(var hex1 in EdgeAdjacentHexes)
                {
                    adj[i++] = new HexEdge(hex0, hex1);
                }
            }

            return adj;
        }

        public HexVertex[] GetVertices()
        {
            HexPosition direction = EdgeAdjacentHexes[1] - EdgeAdjacentHexes[0];
            int directionIndex = Directions.IndexOf(direction);

            return new HexVertex[] {
                new HexVertex(EdgeAdjacentHexes[0], directionIndex),
                new HexVertex(EdgeAdjacentHexes[0], directionIndex + 1),
            };
        }


        public int CompareTo(HexEdge other)
        {
            return GetHashCode() - other.GetHashCode();
        }

        public override int GetHashCode() => hash;

        public static bool operator ==(HexEdge a, HexEdge b) => a.EdgeAdjacentHexes[0] == b.EdgeAdjacentHexes[0] && a.EdgeAdjacentHexes[1] == b.EdgeAdjacentHexes[1];
        public static bool operator !=(HexEdge a, HexEdge b) => a.EdgeAdjacentHexes[0] != b.EdgeAdjacentHexes[0] || a.EdgeAdjacentHexes[1] != b.EdgeAdjacentHexes[1];

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override string ToString()
        {
            return $"[{EdgeAdjacentHexes[0]},{EdgeAdjacentHexes[1]}]";
        }
    }
}
