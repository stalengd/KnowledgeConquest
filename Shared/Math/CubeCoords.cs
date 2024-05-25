using System;

namespace KnowledgeConquest.Shared.Math
{
    public struct CubeCoords
    {
        public int Q { get; set; }
        public int R { get; set; }
        public int S { get; set; }
        public readonly bool IsValid => Q + R + S == 0;
        public static CubeCoords Zero => new CubeCoords(0, 0, 0);

        public CubeCoords(int q, int r, int s)
        {
            Q = q;
            R = r;
            S = s;
            if (!IsValid) throw new ArgumentException("Not valid cube coordinates");
        }

        public readonly OffsetCoords ToOffsetCoords()
        {
            return HexConvert.CubeToOffset(this);
        }

        public static CubeCoords FromOffsetCoords(OffsetCoords v)
        {
            return HexConvert.OffsetToCube(v);
        }

        public override readonly bool Equals(object obj)
        {
            return obj is CubeCoords coords &&
                   Q == coords.Q &&
                   R == coords.R &&
                   S == coords.S;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Q, R, S);
        }

        public override readonly string ToString()
        {
            return $"(Q: {Q}, R: {R}, S: {S})";
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
            return new CubeCoords(left.Q + right.Q, left.R + right.R, left.S + right.S);
        }

        public static CubeCoords operator *(CubeCoords v, int factor) 
        {
            return new CubeCoords(v.Q * factor, v.R * factor, v.S * factor);
        }
    }
}
