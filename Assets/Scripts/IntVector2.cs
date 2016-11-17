namespace Game.HexLines
{
    public struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object other)
        {
            if (!(other is IntVector2))
            {
                return false;
            }
            IntVector2 vector = (IntVector2)other;
            return this.x.Equals(vector.x) && this.y.Equals(vector.y);
        }

        public override int GetHashCode()
        {
            // from http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode/263416#263416
            unchecked // Overflow is fine, just wrap
            {
                int hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = (hash * 16777619) ^ x.GetHashCode();
                hash = (hash * 16777619) ^ y.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(IntVector2 lhs, IntVector2 rhs) { return lhs.x == rhs.x && lhs.y == rhs.y; }
        public static bool operator !=(IntVector2 lhs, IntVector2 rhs) { return lhs.x != rhs.x || lhs.y != rhs.y; }
    }
}