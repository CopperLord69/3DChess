namespace chessEngine {
    public struct Position {
        public int x;
        public int y;

        public static bool operator ==(Position a, Position b) {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Position a, Position b) {
            return !(a == b);
        }
    }
}