namespace chessEngine {
    public struct DirectionParameters {
        public Position start;
        public Position offset;
        public int distance;

        public static DirectionParameters Up(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = 0, y = 1 },
                distance = distance
            };
        }

        public static DirectionParameters Down(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = 0, y = -1 },
                distance = distance
            };
        }

        public static DirectionParameters Left(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = -1, y = 0 },
                distance = distance
            };
        }

        public static DirectionParameters Right(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = 1, y = 0 },
                distance = distance
            };
        }

        public static DirectionParameters UpLeft(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = -1, y = 1 },
                distance = distance
            };
        }

        public static DirectionParameters UpRight(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = 1, y = 1 },
                distance = distance
            };
        }

        public static DirectionParameters DownLeft(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = -1, y = -1 },
                distance = distance
            };
        }

        public static DirectionParameters DownRight(Position start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Position { x = 1, y = -1 },
                distance = distance
            };
        }
    }
}