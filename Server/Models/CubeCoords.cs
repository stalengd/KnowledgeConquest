namespace KnowledgeConquest.Server.Models
{
    public struct CubeCoords
    {
        public int Q { get; set; }
        public int R { get; set; }
        public int S { get; set; }
        public readonly bool IsValid => Q + R + S == 0;
        public static CubeCoords Zero => new(0, 0, 0);

        public CubeCoords(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
            if (!IsValid) throw new ArgumentException("Not valid cube coordinates");
        }

        public readonly Vector2Int ToOffsetCoords()
        {
            var col = Q;
            var row = R + (Q - (Q & 1)) / 2;
            return new Vector2Int(col, row);
        }

        public static CubeCoords FromOffsetCoords(Vector2Int v)
        {
            var q = v.X;
            var r = v.Y - (v.X - (v.X & 1)) / 2;
            return new CubeCoords(q, r, -q - r);
        }

        public override bool Equals(object? obj)
        {
            return obj is CubeCoords coords &&
                   Q == coords.Q &&
                   R == coords.R &&
                   S == coords.S;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Q, R, S);
        }

        public static bool operator ==(CubeCoords left, CubeCoords right)
        {
            return left.Q == right.Q && left.R == right.R && left.S == right.S;
        }

        public static bool operator !=(CubeCoords left, CubeCoords right)
        {
            return !(left == right);
        }

        public static CubeCoords operator +(CubeCoords left, CubeCoords right)
        {
            return new(left.Q + right.Q, left.R + right.R, left.S + right.S);
        }

        public static CubeCoords operator *(CubeCoords v, int factor) 
        {
            return new(v.Q * factor, v.R * factor, v.S * factor);
        }
    }
}
