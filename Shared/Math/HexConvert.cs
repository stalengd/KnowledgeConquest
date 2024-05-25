using System.Collections.Generic;

namespace KnowledgeConquest.Shared.Math
{
    public static class HexConvert
    {
        public static OffsetCoords CubeToOffset(CubeCoords c)
        {
            var col = c.Q + (c.R - (c.R & 1)) / 2;
            var row = c.R;
            return new OffsetCoords(col, row);
        }

        public static CubeCoords OffsetToCube(OffsetCoords v)
        {
            var q = v.X - (v.Y - (v.Y & 1)) / 2;
            var r = v.Y;
            return new CubeCoords(q, r, -q - r);
        }

        public static IEnumerable<OffsetCoords> ToOffsetCoords(this IEnumerable<CubeCoords> cube)
        {
            foreach (var c in cube)
            {
                yield return c.ToOffsetCoords();
            }
        }

    }
}
