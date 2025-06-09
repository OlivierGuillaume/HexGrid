using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGrid
{
    public class HexGrid<TCell> : IEnumerable<HexPosition>
    {
        private readonly Dictionary<HexPosition, TCell> cells = new();

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

        public TCell this[HexPosition position]
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
