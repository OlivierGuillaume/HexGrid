using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGrid
{
    public class HexMap<TTile, TEntity> : IEnumerable<(HexPosition, TTile, TEntity)>
    {

        private Dictionary<HexPosition, Cell> cells;

        public HexMap(Dictionary<HexPosition, TTile> tiles, Dictionary<HexPosition, TEntity> entities)
        {
            cells = new();

            foreach ((HexPosition pos, TTile tile) in tiles)
            {
                cells[pos] = new Cell(tile);
            }

            foreach ((HexPosition pos, TEntity entity) in entities)
            {
                if(cells.TryGetValue(pos, out Cell cell)) cell.entity = entity;
                else cells[pos] = new Cell(entity);
            }
        }

        public HexMap(TTile[,] offsetGrid)
        {
            cells = new();
            for (int x = 0; x < offsetGrid.GetLength(0); x++)
            {
                for (int y = 0; y < offsetGrid.GetLength(1); y++)
                {
                    cells[new(x,y,offsetCoordinates: true)] = new Cell(offsetGrid[x,y]);
                }
            }
        }

        public Cell this[HexPosition position]
        {
            get
            {
                cells.TryGetValue(position, out Cell cell);

                return cell;
            }   
        }

        public HashSet<Cell> this[ICollection<HexPosition> positions]
        {
            get
            {
                HashSet<Cell> results = new();
                foreach(HexPosition pos in positions)
                {
                    results.Add(cells[pos]);
                }
                return results;
            }
        }

        public IEnumerator<(HexPosition, TTile, TEntity)> GetEnumerator()
        {
            foreach((HexPosition pos, Cell cell) in cells)
                yield return (pos, cell.tile, cell.entity);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach ((HexPosition pos, Cell cell) in cells)
                yield return (pos, cell.tile, cell.entity);
        }

        public struct Cell
        {
            public TTile tile;
            public TEntity entity;

            public Cell(TTile tile)
            {
                this.tile = tile;
                this.entity = default;
            }

            public Cell(TEntity entity)
            {
                this.tile = default;
                this.entity = entity;
            }

            public Cell(TTile tile, TEntity entity)
            {
                this.tile = tile;
                this.entity = entity;
            }
        }

    }
}
