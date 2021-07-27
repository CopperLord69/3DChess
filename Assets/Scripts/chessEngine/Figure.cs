namespace chessEngine {
    public struct Figure {
        public int movesCount;
        public bool madeMoveJustNow;
        public bool isWhite;
        public FigureType figureType;

        public static bool operator ==(Figure a, Figure b) {
            return a.figureType == b.figureType
                && a.madeMoveJustNow == b.madeMoveJustNow
                && a.movesCount == b.movesCount;
        }

        public static bool operator !=(Figure a, Figure b) {
            return !(a == b);
        }
    }
}