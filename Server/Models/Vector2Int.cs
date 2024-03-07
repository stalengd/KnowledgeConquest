namespace KnowledgeConquest.Server.Models
{
    public struct Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2Int() 
        {
            X = 0;
            Y = 0;
        }
        
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(Vector2Int left, Vector2Int right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Vector2Int left, Vector2Int right)
        {
            return !(left == right);
        }

        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
        {
            return new(left.X + right.X, left.Y + right.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2Int @int &&
                   X == @int.X &&
                   Y == @int.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}
