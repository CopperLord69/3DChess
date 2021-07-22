using option;
using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public static class Chess {

        public static Dictionary<Position, ChessFigure> GetCastlingPositions(
            ChessFigure figure,
            List<ChessFigure> figures
            ) {
            Dictionary<Position, ChessFigure> castlingPositions = CalculateCastlings(
                figure,
                figures
                );
            Dictionary<Position, ChessFigure> castlingPositionsToReturn =
                new Dictionary<Position, ChessFigure>();
            if (figure.movesCount == 0) {
                foreach (var position in castlingPositions) {
                    if (castlingPositions[position.Key].movesCount != 0) {
                        continue;
                    }
                    var allies = GetAllies(figure.color, figures);
                    int rowFigureCount = 0;
                    foreach (var ally in allies) {
                        if (ally.position.y == figure.position.y) {
                            if (position.Key.x == 2) {
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
                    if (rowFigureCount == 0 && position.Key.y == figure.position.y) {
                        castlingPositionsToReturn.Add(position.Key, position.Value);
                    }
                }
            }
            return castlingPositionsToReturn;
        }

        private static Dictionary<Position, ChessFigure> CalculateCastlings(
            ChessFigure figure,
            List<ChessFigure> figures
            ) {
            Dictionary<Position, ChessFigure> castlingPositions =
                new Dictionary<Position, ChessFigure>();
            Position leftRookPosition;
            Position rightRookPosition;
            Position leftKingPosition;
            Position rightKingPosition;
            int y;
            if (figure.color == FigureColor.Black) {
                y = 7;
            } else {
                y = 0;
            }
            leftRookPosition = new Position(0, y);
            leftKingPosition = new Position(2, y);
            rightRookPosition = new Position(7, y);
            rightKingPosition = new Position(6, y);
            foreach (var fig in figures) {
                if (Position.AreSame(fig.position, leftRookPosition)) {
                    castlingPositions.Add(leftKingPosition, fig);
                }
                if (Position.AreSame(fig.position, rightRookPosition)) {
                    castlingPositions.Add(rightKingPosition, fig);
                }
            }
            return castlingPositions;
        }

        public static Dictionary<Position, ChessFigure> GetPawnElPassants(
            ChessFigure figure,
            List<ChessFigure> figures
        ) {
            Dictionary<Position, ChessFigure> approachPositions = new Dictionary<Position, ChessFigure>(); ;
            var enemies = GetEnemies(figure.color, figures);
            int enemyFirstMoveLine;
            int xOffset;
            if (figure.color == FigureColor.White) {
                enemyFirstMoveLine = 4;
                xOffset = 1;
            } else {
                enemyFirstMoveLine = 3;
                xOffset = -1;
            }
            foreach (var enemy in enemies) {
                if (
                    enemy.position.y == enemyFirstMoveLine
                    && enemy.position.y == figure.position.y
                    ) {
                    if (Mathf.Abs(enemy.position.x - figure.position.x) < 2) {
                        bool isPawnWithOneDouble = (enemy.type == Figure.Pawn);
                        isPawnWithOneDouble &= enemy.movesCount == 1;
                        isPawnWithOneDouble &= enemy.madeTurnJustNow;
                        if (isPawnWithOneDouble) {
                            var pos = enemy.position;
                            pos.y += xOffset;
                            approachPositions.Add(pos, enemy);
                        }
                    }
                }
            }
            return approachPositions;
        }

        public static bool NoAvailableMoves(FigureColor color, List<ChessFigure> figures) {
            var allies = GetAllies(color, figures);
            foreach (var ally in allies) {
                var allyMoveDeirections = CalculateMoveDirections(ally, figures);
                if (allyMoveDeirections.Count != 0) {
                    return false;
                }
            }
            return true;
        }

        public static bool IsKingInDanger(ChessFigure king, List<ChessFigure> figures) {
            List<ChessFigure> enemies = GetEnemies(king.color, figures);
            foreach (var enemy in enemies) {
                var enemyMoveDirecitons = CalculateFigureMoveDirections(enemy, figures);
                foreach (var enemyDirection in enemyMoveDirecitons) {
                    if (enemyDirection.Contains(king.position)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckForCheck(List<ChessFigure> figures) {
            var kings = GetKings(figures);
            foreach (var king in kings) {
                if (IsKingInDanger(king, figures)) {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckForMate(List<ChessFigure> figures) {
            var kings = GetKings(figures);
            foreach (var king in kings) {
                if (IsKingInDanger(king, figures)) {
                    if (NoAvailableMoves(king.color, figures)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<ChessFigure> GetEnemies(FigureColor color, List<ChessFigure> figures) {
            List<ChessFigure> enemies = new List<ChessFigure>();
            foreach (var figure in figures) {
                if (figure.color != color) {
                    enemies.Add(figure);
                }
            }
            return enemies;
        }

        public static List<ChessFigure> GetAllies(FigureColor color, List<ChessFigure> figures) {
            List<ChessFigure> allies = new List<ChessFigure>();
            foreach (var figure in figures) {
                if (figure.color == color) {
                    allies.Add(figure);
                }
            }
            return allies;
        }

        public static List<ChessFigure> GetKings(List<ChessFigure> figures) {
            var kings = new List<ChessFigure>();
            foreach (var figure in figures) {
                if (figure.type == Figure.King) {
                    kings.Add(figure);
                }
            }
            return kings;
        }

        public static Option<ChessFigure> FindKingWithColor(FigureColor color, List<ChessFigure> figures) {
            foreach (var figure in figures) {
                if (figure.type == Figure.King && figure.color == color) {
                    return Option<ChessFigure>.Some(figure);
                }
            }
            return Option<ChessFigure>.None();
        }

        public static List<List<Position>> CalculateMoveDirections(
            ChessFigure figure,
            List<ChessFigure> figures
        ) {
            var allyKing = FindKingWithColor(figure.color, figures);
            var enemies = GetEnemies(figure.color, figures);
            var impossiblePositions = new List<Position>();
            Position initialPosition = figure.position;
            var figureMoveDirections = CalculateFigureMoveDirections(figure, figures);
            foreach (var direction in figureMoveDirections) {
                foreach (var position in direction) {
                    if (allyKing.IsSome()) {
                        List<ChessFigure> tempEnemies = new List<ChessFigure>();
                        foreach(var enemy in enemies) {
                            if(Position.AreSame(position, enemy.position)) {
                                figures.Remove(enemy);
                                tempEnemies.Add(enemy);
                            }
                        }
                        figure.position = position;
                        if (IsKingInDanger(allyKing.Peel(), figures)) {
                            impossiblePositions.Add(position);
                        }
                        foreach(var enemy in tempEnemies) {
                            figures.Add(enemy);
                        }
                    }
                }
                foreach (var position in impossiblePositions) {
                    direction.Remove(position);
                }
            }
            figure.position = initialPosition;
            figureMoveDirections = RemoveEmptyDirections(figureMoveDirections);
            return figureMoveDirections;
        }

        private static List<List<Position>> CalculateFigureMoveDirections(
            ChessFigure figure,
            List<ChessFigure> figures
        ) {
            var moveDirections = new List<List<Position>>();
            int x = figure.position.x;
            int y = figure.position.y;
            switch (figure.type) {
                case Figure.Pawn: {
                        var enemyFigures = figures.FindAll(other => other.color != figure.color);
                        var directionParameters = new DirectionParameters();
                        directionParameters.distance = 1;
                        directionParameters.start.x = x;
                        directionParameters.start.y = y;
                        int firstMoveLine;
                        if (figure.color == FigureColor.Black) {
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
                            if (!PositionsHasEnemyPosition(direction, enemyFigures)) {
                                direction.Clear();
                            }
                        }
                        directionParameters.offset.x = 0;
                        var positions = CalculateDirection(directionParameters);
                        if (!PositionsHasEnemyPosition(positions, enemyFigures)) {
                            moveDirections.Add(positions);
                            if (figure.position.y == firstMoveLine) {
                                directionParameters.distance = 2;
                                var farPosition = CalculateDirection(directionParameters);
                                if (!PositionsHasEnemyPosition(farPosition, enemyFigures)) {
                                    moveDirections.Add(farPosition);
                                }
                            }
                        }
                        break;
                    }
                case Figure.Bishop: {
                        int distance = 8;

                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpLeft(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpRight(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownLeft
                                (new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownRight(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        break;
                    }
                case Figure.Rook: {
                        int distance = 8;
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Up(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Down(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Left(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Right(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        break;
                    }
                case Figure.Queen: {
                        int distance = 8;
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Up(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Down(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Left(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Right(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                           CalculateDirection(
                               DirectionParameters.UpLeft(
                                   new Vector2Int(x, y), distance)
                               )
                           );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpRight(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownLeft
                                (new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownRight(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        break;
                    }
                case Figure.Knight: {
                        int distance = 1;
                        var directionParameters = new DirectionParameters();
                        directionParameters.distance = distance;
                        directionParameters.start = new Vector2Int(x, y);
                        directionParameters.offset = new Vector2Int(2, 1);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Vector2Int(2, -1);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Vector2Int(-2, 1);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Vector2Int(-2, -1);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Vector2Int(1, 2);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Vector2Int(-1, 2);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Vector2Int(1, -2);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        directionParameters.offset = new Vector2Int(-1, -2);
                        moveDirections.Add(CalculateDirection(directionParameters));
                        break;
                    }
                case Figure.King: {
                        int distance = 1;
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Up(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Down(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Left(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.Right(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                           CalculateDirection(
                               DirectionParameters.UpLeft(
                                   new Vector2Int(x, y), distance)
                               )
                           );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.UpRight(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownLeft
                                (new Vector2Int(x, y), distance)
                                )
                            );
                        moveDirections.Add(
                            CalculateDirection(
                                DirectionParameters.DownRight(
                                    new Vector2Int(x, y), distance)
                                )
                            );
                        break;
                    }
                default: {
                        break;
                    }
            }
            moveDirections = TraceMoveDirections(moveDirections, figures);
            moveDirections = RemoveAllyPositions(figure, moveDirections, figures);
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

        private static List<Position> CalculateDirection(DirectionParameters parameters) {
            List<Position> direction = new List<Position>();
            var positionX = parameters.start.x + parameters.offset.x;
            var positionY = parameters.start.y + parameters.offset.y;
            for (int i = 0; i < parameters.distance; i++) {
                if (positionX < 0 || positionX >= 8
                    || positionY < 0 || positionY >= 8) {
                    break;
                }
                direction.Add(new Position(positionX, positionY));
                positionX += parameters.offset.x;
                positionY += parameters.offset.y;
            }
            return direction;
        }

        private static bool PositionsHasEnemyPosition(
            List<Position> positions,
            List<ChessFigure> enemies
        ) {
            foreach (var enemy in enemies) {
                if (positions.Contains(enemy.position)) {
                    return true;
                }
            }
            return false;
        }

        public static Option<ChessFigure> GetCollidingEnemy(
            ChessFigure figure,
            List<ChessFigure> figures
        ) {
            var enemies = GetEnemies(figure.color, figures);
            foreach (var enemy in enemies) {
                if (Position.AreSame(enemy.position, figure.position)) {
                    return Option<ChessFigure>.Some(enemy);
                }
            }
            return Option<ChessFigure>.None();
        }

        private static List<List<Position>> TraceMoveDirections(
            List<List<Position>> moveDirections,
            List<ChessFigure> figures
        ) {
            List<Position> otherFiguresPositions = new List<Position>();
            foreach (var otherFigure in figures) {
                otherFiguresPositions.Add(otherFigure.position);
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
            return moveDirections;
        }

        private static List<List<Position>> RemoveAllyPositions(
            ChessFigure figure,
            List<List<Position>> moveDirections,
            List<ChessFigure> figures
        ) {
            List<ChessFigure> allyFigures = new List<ChessFigure>();
            foreach (var fig in figures) {
                if (fig.color == figure.color && fig != figure) {
                    allyFigures.Add(fig);
                }
            }
            foreach (var direciton in moveDirections) {
                foreach (var ally in allyFigures) {
                    direciton.Remove(ally.position);
                }
            }
            return moveDirections;
        }

        private static List<List<Position>> RemoveEmptyDirections(List<List<Position>> list) {
            List<List<Position>> emptyDirections = new List<List<Position>>();
            foreach (var direction in list) {
                if (direction.Count == 0) {
                    emptyDirections.Add(direction);
                }
            }
            foreach (var emptyDirection in emptyDirections) {
                list.Remove(emptyDirection);
            }
            return list;
        }

        public static List<ChessFigure> RemoveNullFigures(List<ChessFigure> figures) {
            List<ChessFigure> nullFigures = new List<ChessFigure>();
            foreach (var figure in figures) {
                if (figure == null) {
                    nullFigures.Add(figure);
                }
            }
            foreach (var nullFigure in nullFigures) {
                figures.Remove(nullFigure);
            }
            return figures;
        }
    }
}