using KnowledgeConquest.Server.Models;

namespace KnowledgeConquest.Server
{
    public static class HexMath
    {
        private static readonly Vector2Int[,] _offsetCoordinatesDirectionOffsets = new Vector2Int[2,6]
        {
            { new(1, 0), new(1, -1), new(0, -1), new(-1, -1), new(-1, 0), new(0, 1) },
            { new(1, 1), new(1, 0), new(0, -1), new(-1, 0), new(-1, 1), new(0, 1) },
        };
        private static readonly CubeCoords[] _cubeCoordinatesDirectionOffsets = new CubeCoords[]
        {
            new(1, 0, -1), new(1, -1, 0), new(0, -1, 1), new(-1, 0, 1), new(-1, 1, 0), new(0, 1, -1)
        };

        public static CubeCoords GetCubeDirection(int direction) => _cubeCoordinatesDirectionOffsets[direction];

        public static Vector2Int GetNeighbour(Vector2Int pos, int direction)
        {
            var parity = pos.X & 1;
            var offset = _offsetCoordinatesDirectionOffsets[parity, direction];
            return pos + offset;
        }

        public static CubeCoords GetNeighbour(CubeCoords pos, int direction)
        {
            return pos + GetCubeDirection(direction);
        }

        public static void Ring(CubeCoords center, int radius, IList<CubeCoords> results)
        {
            var cur = center + GetCubeDirection(4) * radius;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    results.Add(cur);
                    cur = GetNeighbour(cur, i);
                }
            }
        }

        public static void Spiral(CubeCoords center, int radius, IList<CubeCoords> results)
        {
            results.Add(center);
            for (int i = 1; i <= radius; i++)
            {
                Ring(center, i, results);
            }
        }

        public static IEnumerable<Vector2Int> ToOffsetCoords(this IEnumerable<CubeCoords> cube)
        {
            foreach (var c in cube)
            {
                yield return c.ToOffsetCoords();
            }
        }
    }
}
