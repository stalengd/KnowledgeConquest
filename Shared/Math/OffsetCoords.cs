using System;

namespace KnowledgeConquest.Shared.Math
{
    public struct OffsetCoords
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public OffsetCoords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(OffsetCoords left, OffsetCoords right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(OffsetCoords left, OffsetCoords right)
        {
            return !(left == right);
        }

        public static OffsetCoords operator +(OffsetCoords left, OffsetCoords right)
        {
            return new OffsetCoords(left.X + right.X, left.Y + right.Y);
        }

        public override bool Equals(object obj)
        {
            return obj is OffsetCoords @int &&
                   X == @int.X &&
                   Y == @int.Y;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
