using System.Collections.Generic;

namespace KnowledgeConquest.Shared.Math
{
    public static class HexMath
    {
        private static readonly OffsetCoords[,] _offsetCoordinatesDirectionOffsets = new OffsetCoords[2,6]
        {
            { new OffsetCoords(1, 0), new OffsetCoords(1, -1), new OffsetCoords(0, -1), new OffsetCoords(-1, -1), new OffsetCoords(-1, 0), new OffsetCoords(0, 1) },
            { new OffsetCoords(1, 1), new OffsetCoords(1, 0), new OffsetCoords(0, -1), new OffsetCoords(-1, 0), new OffsetCoords(-1, 1), new OffsetCoords(0, 1) },
        };
        private static readonly CubeCoords[] _cubeCoordinatesDirectionOffsets = new CubeCoords[]
        {
            new CubeCoords(1, 0, -1), new CubeCoords(1, -1, 0), new CubeCoords(0, -1, 1), new CubeCoords(-1, 0, 1), new CubeCoords(-1, 1, 0), new CubeCoords(0, 1, -1)
        };

        public static CubeCoords GetCubeDirection(int direction) => _cubeCoordinatesDirectionOffsets[direction];

        public static OffsetCoords GetNeighbour(OffsetCoords pos, int direction)
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
    }
}
