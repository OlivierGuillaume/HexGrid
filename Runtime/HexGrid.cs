using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexGrid
{
    public class HexGrid<TCell> : IEnumerable<HexPosition>
    {
        protected readonly Dictionary<HexPosition, TCell> cells = new();

        public void Clear() => cells.Clear();

        public int CellsCount => cells.Count;

        public HexGrid(Dictionary<HexPosition, TCell> values)
        {
            foreach ((HexPosition pos, TCell value) in values)
            {
                this.cells[pos] = value;
            }
        }


        public HexGrid(){}

        public HexGrid(TCell[,] offsetGrid, bool createCellForNull = true)
        {
            for (int x = 0; x < offsetGrid.GetLength(0); x++)
            {
                for (int y = 0; y < offsetGrid.GetLength(1); y++)
                {
                    TCell value = offsetGrid[x, y];

                    if(createCellForNull || value != null)
                        cells[new(x,y,offsetCoordinates: true)] = offsetGrid[x,y];
                }
            }
        }

        public HexGrid(Vector2Int offsetSize)
        {
            cells = new();
            for (int x = 0; x < offsetSize.x; x++)
            {
                for (int y = 0; y < offsetSize.y; y++)
                {
                    cells[new(x, y, offsetCoordinates: true)] = default;
                }
            }
        }

        public virtual TCell this[HexPosition position]
        {
            get
            {
                cells.TryGetValue(position, out TCell val);

                return val;
            }   

            set
            {
                cells[position] = value;
            }
        }

        public bool TryGet(HexPosition position, out TCell val) => cells.TryGetValue(position, out val);


        public HashSet<TCell> this[ICollection<HexPosition> positions]
        {
            get
            {
                HashSet<TCell> results = new();
                foreach(HexPosition pos in positions)
                {
                    results.Add(this[pos]);
                }
                return results;
            }
        }

        public IEnumerator<HexPosition> GetEnumerator()
        {
            return cells.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cells.Keys.GetEnumerator();
        }

        public List<List<HexVertex>> GetBorders(Func<HexPosition, bool> IsInside)
        {
            return GetBorders(cells.Keys.Where(pos => IsInside(pos)).ToHashSet());
        }
        public List<List<HexVertex>> GetBorders(HashSet<HexPosition> territory)
        {
            HashSet<HexVertex> visited = new();
            List<List<HexVertex>> borders = new();


            foreach (HexPosition pos in territory)
            {
                if (!IsHexBorder(pos)) continue;

                foreach (HexVertex v in pos.GetAjactentVertices())
                {
                    if(visited.Contains(v)) continue;

                    if (!IsVerticeBorder(v)) continue;

                    borders.Add(FollowBorderFrom(v));
                }
            }

            return borders;

            List<HexVertex> FollowBorderFrom(HexVertex v)
            {
                List<HexVertex> border = new();
                visited.Add(v);
                border.Add(v);

                HexVertex next = default;
                bool nextFound = true;

                while (nextFound)
                {
                    nextFound = false;
                    foreach (HexVertex v2 in v.GetAdjacentVertices())
                    {
                        if (visited.Contains(v2) || !IsEdgeBorder(new(v,v2)))
                            continue;

                        visited.Add(v2);
                        next = v2;
                        nextFound = true;
                        border.Add(next);
                        break;
                    }
                    v = next;
                }

                return border;
            }

            bool IsEdgeBorder(HexEdge e)
            {
                var hexes = e.GetEdgeAdjacentHexes();

                return territory.Contains(hexes[0]) != territory.Contains(hexes[1]);
            }
            
            bool IsHexBorder(HexPosition pos)
            {
                foreach(HexPosition adj in pos.GetAdjacentHexes())
                {
                    if (!territory.Contains(adj)) return true;
                }

                return false;
            }

            bool IsVerticeBorder(HexVertex v)
            {
                foreach (HexPosition adj in v.GetAdjacentHexes())
                {
                    if (!territory.Contains(adj)) return true;
                }

                return false;
            }
        }

        public Bounds CalculateBounds()
        {
            Bounds bounds = default;
            bool first = true;

            foreach(var hex in this)
            {
                if (first)
                {
                    first = false;
                    bounds = new Bounds(hex.WorldPosition, Vector3.zero);
                    continue;
                }

                bounds.Encapsulate(hex.WorldPosition);
            }

            return bounds;
        }
    }

    public class HexGrid<TCell, TEdge, TVertex> : HexGrid<TCell>
    {
        private readonly Dictionary<HexEdge, TEdge> edges = new();
        private readonly Dictionary<HexVertex, TVertex> vertices = new();

        public TEdge this[HexEdge position]
        {
            get
            {
                edges.TryGetValue(position, out TEdge val);

                return val;
            }

            set
            {
                edges[position] = value;
            }
        }

        public HashSet<TEdge> this[ICollection<HexEdge> positions]
        {
            get
            {
                HashSet<TEdge> results = new();
                foreach (HexEdge pos in positions)
                {
                    results.Add(this[pos]);
                }
                return results;
            }
        }

        public TVertex this[HexVertex position]
        {
            get
            {
                vertices.TryGetValue(position, out TVertex val);

                return val;
            }

            set
            {
                vertices[position] = value;
            }
        }

        public HashSet<TVertex> this[ICollection<HexVertex> positions]
        {
            get
            {
                HashSet<TVertex> results = new();
                foreach (HexVertex pos in positions)
                {
                    results.Add(this[pos]);
                }
                return results;
            }
        }

        public IEnumerator<HexVertex> Vertices => vertices.Keys.GetEnumerator();
        public IEnumerator<HexEdge> Edges => edges.Keys.GetEnumerator();
    }
}
