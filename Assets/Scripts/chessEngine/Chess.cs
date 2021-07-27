using System;
using System.Collections.Generic;

namespace chessEngine {

    public static class Chess {

        public static List<Position> CalculateMovePositions(
            Position figurePosition,
            List<List<Field>> board
        ) {
            var field = board[figurePosition.x][figurePosition.y];
            var figureMovePositions = new List<Position>();
            if (field.isFilled) {
                var allyKing = FindKingWithColor(field.figure.isWhite, board);
                var impossiblePositions = new List<Position>();
                var figureMoveDirections = GetMovePositions(field.position, board);
                foreach (var direction in figureMoveDirections) {
                    foreach (var position in direction) {
                        if (allyKing.isFilled) {
                            var tempOldPosition = field;
                            var tempNewPosition = board[position.x][position.y];
                            var tempOldPos = tempOldPosition.position;
                            field = tempNewPosition;
                            board[position.x][position.y] = tempOldPosition;
                            board[position.x][position.y].position = position;
                            if (IsKingInDanger(allyKing, board)) {
                                impossiblePositions.Add(position);
                            }
                            field = tempOldPosition;
                            board[position.x][position.y] = tempNewPosition;
                            field.position = tempOldPos;
                        }
                    }
                    foreach (var position in impossiblePositions) {
                        direction.Remove(position);
                    }
                }
                foreach (var figureDirection in figureMoveDirections) {
                    figureMovePositions.AddRange(figureDirection);
                }
            }
            return figureMovePositions;
        }

        public static GameSituation GetGameSituation(bool isWhite, List<List<Field>> board) {
            foreach (var row in board) {
                foreach (var field in row) {
                    if (
                        field.figure.figureType == FigureType.King 
                        && field.figure.isWhite == isWhite
                    ) {
                        if (IsKingInDanger(field, board)) {
                            var allies = GetFiguresWithSameColor(isWhite, board);
                            foreach (var ally in allies) {
                                var allyMoveDeirections = 
                                    CalculateMovePositions(ally.position, board);
                                if (allyMoveDeirections.Count != 0) {
                                    return GameSituation.Check;
                                }
                            }
                            return GameSituation.Mate;
                        } else {
                            var allies = GetFiguresWithSameColor(isWhite, board);
                            foreach (var ally in allies) {
                                var allyMoveDeirections = 
                                    CalculateMovePositions(ally.position, board);
                                if (allyMoveDeirections.Count != 0) {
                                    return GameSituation.Nothing;
                                }
                            }
                            return GameSituation.Stalemate;
                        }
                    }
                }
            }
            return GameSituation.Nothing;
        }

        public static Dictionary<Position, Position> GetCastlingPositions(
            Position kingPosition,
            List<List<Field>> board
        ) {
            var king = board[kingPosition.x][kingPosition.y];
            Dictionary<Position, Position> castlingPositionsToReturn =
                new Dictionary<Position, Position>();
            if (king.isFilled) {
                Dictionary<Position, Position> castlingPositions =
                new Dictionary<Position, Position>();
                Position leftRookPosition;
                Position rightRookPosition;
                Position leftKingPosition;
                Position rightKingPosition;
                int y;
                if (!king.figure.isWhite) {
                    y = 7;
                } else {
                    y = 0;
                }
                leftRookPosition = new Position { x = 0, y = y };
                leftKingPosition = new Position { x = 2, y = y };
                rightRookPosition = new Position { x = 7, y = y };
                rightKingPosition = new Position { x = 6, y = y };
                castlingPositions.Add(leftKingPosition, leftRookPosition);
                castlingPositions.Add(rightKingPosition, rightRookPosition);
                if (king.figure.movesCount == 0) {
                    foreach (var pos in castlingPositions) {
                        var ps = castlingPositions[pos.Key];
                        var figure = board[ps.x][ps.y];
                        if (figure.isFilled) {
                            if (figure.figure.movesCount != 0) {
                                continue;
                            }
                        } else {
                            continue;
                        }
                        var allies = GetFiguresWithSameColor(king.figure.isWhite, board);
                        int rowFigureCount = 0;
                        foreach (var ally in allies) {
                            if (ally.position.y == king.position.y) {
                                if (pos.Key.x == 2) {
                                    if (ally.position.x > 0 && ally.position.x < 4) {
                                        rowFigureCount++;
                                        break;
                                    }
                                } else {
                                    if (ally.position.x > 4 && ally.position.x < 7) {
                                        rowFigureCount++;
                                        break;
                                    }
                                }
                            }
                        }
                        if (rowFigureCount == 0 && pos.Key.y == king.position.y) {
                            castlingPositionsToReturn.Add(pos.Key, pos.Value);
                        }
                    }
                }
            }

            return castlingPositionsToReturn;
        }

        public static Dictionary<Position, Position> GetPawnEnPassants(
           Position pawnPosition,
           List<List<Field>> board
       ) {
            Dictionary<Position, Position> approachPositions = new Dictionary<Position, Position>();
            var pawn = board[pawnPosition.x][pawnPosition.y];
            if (pawn.isFilled) {
                var enemies = GetFiguresWithOppositeColor(pawn.figure.isWhite, board);
                int enemyFirstMoveLine;
                int xOffset;
                if (pawn.figure.isWhite) {
                    enemyFirstMoveLine = 4;
                    xOffset = 1;
                } else {
                    enemyFirstMoveLine = 3;
                    xOffset = -1;
                }
                foreach (var enemy in enemies) {
                    if (
                        enemy.position.y == enemyFirstMoveLine
                        && enemy.position.y == pawn.position.y
                        ) {
                        if (Math.Abs(enemy.position.x - pawn.position.x) < 2) {
                            bool isPawnWithOneDouble = (enemy.figure.figureType == FigureType.Pawn);
                            isPawnWithOneDouble &= enemy.figure.movesCount == 1;
                            isPawnWithOneDouble &= enemy.figure.madeMoveJustNow;
                            if (isPawnWithOneDouble) {
                                var pos = enemy.position;
                                pos.y += xOffset;
                                approachPositions.Add(pos, enemy.position);
                            }
                        }
                    }
                }
            }
            return approachPositions;
        }

        private static bool IsKingInDanger(Field king, List<List<Field>> board) {
            List<Field> enemies = GetFiguresWithOppositeColor(king.figure.isWhite, board);
            foreach (var enemy in enemies) {
                var enemyMoveDirecitons = GetMovePositions(enemy.position, board);
                foreach (var enemyDirection in enemyMoveDirecitons) {
                    if (enemyDirection.Contains(king.position)) {
                        return true;
                    }
                }
            }
            return false;
        }

        private static Field FindKingWithColor(bool isWhite, List<List<Field>> board) {
            foreach (var row in board) {
                foreach (var field in row) {
                    if (field.isFilled) {
                        bool isValidKing = field.figure.figureType == FigureType.King;
                        isValidKing &= field.figure.isWhite == isWhite;
                        if (isValidKing) {
                            return field;
                        }
                    }
                }
            }
            return Field.Empty();
        }

        private static List<List<Position>> GetMovePositions(
            Position figurePosition,
            List<List<Field>> board
        ) {
            var moveDirections = new List<List<Position>>();
            int x = figurePosition.x;
            int y = figurePosition.y;
            Field field = board[x][y];
            switch (field.figure.figureType) {
                case FigureType.Pawn: {
                        var enemyFigures = GetFiguresWithOppositeColor(field.figure.isWhite, board);
                        var directionParameters = new DirectionParameters {
                            distance = 1,
                            start = new Position { x = x, y = y }
                        };
                        int firstMoveLine;
                        if (!field.figure.isWhite) {
                            directionParameters.offset.y = -1;
                            firstMoveLine = 6;
                        } else {
                            directionParameters.offset.y = 1;
                            firstMoveLine = 1;
                        }
                        directionParameters.offset.x = 1;
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset.x = -1;
                        moveDirections.Add(CalculateDirection(directionParameters));
                        foreach (var direction in moveDirections) {
                            if (!HasEnemyOnDirection(direction, enemyFigures)) {
                                direction.Clear();
                            }
                        }
                        directionParameters.offset.x = 0;
                        var positions = CalculateDirection(directionParameters);
                        if (!HasEnemyOnDirection(positions, enemyFigures)) {
                            moveDirections.Add(positions);
                            if (field.position.y == firstMoveLine) {
                                directionParameters.distance = 2;
                                var farPosition = CalculateDirection(directionParameters);
                                if (!HasEnemyOnDirection(farPosition, enemyFigures)) {
                                    moveDirections.Add(farPosition);
                                }
                            }
                        }
                        foreach (var key in GetPawnEnPassants(field.position, board).Keys) {
                            moveDirections.Add(new List<Position> { key });
                        }
                        break;
                    }
                case FigureType.Bishop: {
                        int distance = 8;

                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpLeft(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpRight(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownLeft
                                (new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownRight(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        break;
                    }
                case FigureType.Rook: {
                        int distance = 8;
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Up(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Down(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Left(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Right(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        break;
                    }
                case FigureType.Queen: {
                        int distance = 8;
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Up(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Down(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Left(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Right(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                           CalculateDirection(
                               DirectionParameters.UpLeft(
                                   new Position { x = x, y = y }, distance)
                               )
                           );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpRight(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownLeft
                                (new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownRight(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        break;
                    }
                case FigureType.Knight: {
                        int distance = 1;
                        var directionParameters = new DirectionParameters();
                        directionParameters.distance = distance;
                        directionParameters.start = new Position { x = x, y = y };
                        directionParameters.offset = new Position { x = 2, y = 1 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Position { x = 2, y = -1 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Position { x = -2, y = 1 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Position { x = -2, y = -1 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Position { x = 1, y = 2 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Position { x = -1, y = 2 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Position { x = 1, y = -2 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Position { x = -1, y = -2 };
                        moveDirections.Add(CalculateDirection(directionParameters));
                        break;
                    }
                case FigureType.King: {
                        int distance = 1;
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Up(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Down(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Left(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Right(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                           CalculateDirection(
                               DirectionParameters.UpLeft(
                                   new Position { x = x, y = y }, distance)
                               )
                           );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpRight(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownLeft
                                (new Position { x = x, y = y }, distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownRight(
                                    new Position { x = x, y = y }, distance)
                                )
                            );
                        var castlings = GetCastlingPositions(field.position, board);
                        foreach (var pos in castlings) {
                            moveDirections.Add(new List<Position> { pos.Key });
                        }
                        break;
                    }
                default: {
                        break;
                    }
            }
            List<Position> otherFiguresPositions = new List<Position>();
            foreach (var row in board) {
                foreach (var fld in row) {
                    if (fld.isFilled) {
                        otherFiguresPositions.Add(fld.position);
                    }
                }
            }
            foreach (var otherFigurePosition in otherFiguresPositions) {
                foreach (var direction in moveDirections) {
                    if (direction.Contains(otherFigurePosition)) {
                        int index = direction.IndexOf(otherFigurePosition);
                        index += 1;
                        direction.RemoveRange(index, direction.Count - index);
                    }
                }
            }
            var allyFigures = GetFiguresWithSameColor(field.figure.isWhite, board);
            foreach (var direciton in moveDirections) {
                foreach (var ally in allyFigures) {
                    direciton.Remove(ally.position);
                }
            }
            List<List<Position>> directionsToRemove = new List<List<Position>>();
            foreach (var direction in moveDirections) {
                if (direction.Count == 0) {
                    directionsToRemove.Add(direction);
                }
            }
            foreach (var direction in directionsToRemove) {
                moveDirections.Remove(direction);
            }
            return moveDirections;
        }

        private static bool HasEnemyOnDirection(
            List<Position> direction,
            List<Field> enemies
        ) {
            foreach (var enemy in enemies) {
                if (direction.Contains(enemy.position)) {
                    return true;
                }
            }
            return false;
        }

        private static List<Position> CalculateDirection(DirectionParameters parameters) {
            List<Position> direction = new List<Position>();
            var positionX = parameters.start.x + parameters.offset.x;
            var positionY = parameters.start.y + parameters.offset.y;
            for (int i = 0; i < parameters.distance; i++) {
                if (positionX < 0 || positionX >= 8
                    || positionY < 0 || positionY >= 8) {
                    break;
                }
                direction.Add(new Position { x = positionX, y = positionY });
                positionX += parameters.offset.x;
                positionY += parameters.offset.y;
            }
            return direction;
        }

        private static List<Field> GetFiguresWithOppositeColor(bool isWhite, List<List<Field>> board) {
            List<Field> enemies = new List<Field>();
            foreach (var row in board) {
                foreach (var field in row) {
                    if (field.isFilled && field.figure.isWhite != isWhite) {
                        enemies.Add(field);
                    }
                }
            }
            return enemies;
        }

        private static List<Field> GetFiguresWithSameColor(bool isWhite, List<List<Field>> board) {
            List<Field> allies = new List<Field>();
            foreach (var row in board) {
                foreach (var field in row) {
                    if (field.isFilled && field.figure.isWhite == isWhite) {
                        allies.Add(field);
                    }
                }
            }
            return allies;
        }
    }
}