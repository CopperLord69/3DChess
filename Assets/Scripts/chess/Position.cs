namespace chessEngn {
    public struct Position {
        public int x;
        public int y;

        public Position(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static bool AreSame(Position a, Position b) {
            return a.x == b.x && a.y == b.y;
        } 
    }
}