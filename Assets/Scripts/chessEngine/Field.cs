namespace chessEngine {
    public class Field {
        public bool isFilled;
        public Position position;
        public Figure figure;

        public static Field Empty() {
            return new Field() {
                position = new Position() {
                    x = -1,
                    y = -1,
                }
            };
        }
    }
}