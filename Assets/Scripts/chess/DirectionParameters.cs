using UnityEngine;

namespace chessEngn {
    public struct DirectionParameters {
        public Vector2Int start;
        public Vector2Int offset;
        public int distance;

        public static DirectionParameters Up(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(0, 1),
                distance = distance
            };
        }

        public static DirectionParameters Down(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(0, -1),
                distance = distance
            };
        }

        public static DirectionParameters Left(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(-1, 0),
                distance = distance
            };
        }

        public static DirectionParameters Right(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(1, 0),
                distance = distance
            };
        }

        public static DirectionParameters UpLeft(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(-1, 1),
                distance = distance
            };
        }

        public static DirectionParameters UpRight(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(1, 1),
                distance = distance
            };
        }

        public static DirectionParameters DownLeft(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(-1, -1),
                distance = distance
            };
        }

        public static DirectionParameters DownRight(Vector2Int start, int distance) {
            return new DirectionParameters {
                start = start,
                offset = new Vector2Int(1, -1),
                distance = distance
            };
        }
    }
}