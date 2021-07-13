using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class FiguresController {

        private List<ChessFigure> figures;

        private Vector2Int[][] boardPositions;

        List<List<Vector2Int>> moveDirections = new List<List<Vector2Int>>();


        public FiguresController(List<ChessFigure> figures, int boardSize) {
            this.figures = figures;
            boardPositions = new Vector2Int[boardSize][];
            for (int i = 0; i < boardSize; i++) {
                boardPositions[i] = new Vector2Int[boardSize];
                for (int j = 0; j < boardSize; j++) {
                    boardPositions[i][j] = new Vector2Int(i, j);
                }
            }
        }

        public List<List<Vector2Int>> CalculateFigureMoveDirections(ChessFigure figure) {
            moveDirections = new List<List<Vector2Int>>();
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
                        if (PositionsHasEnemyPosition(positions, enemyFigures)) {
                            directionParameters.offset.x = 1;
                            moveDirections.Add(CalculateDirection(directionParameters));
                            directionParameters.offset.x = -1;
                            moveDirections.Add(CalculateDirection(directionParameters));
                        } else {
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
            figures.RemoveAll(figure => figure == null);
            TraceMoveDirections();
            RemoveAllyPositions(figure);
            moveDirections.RemoveAll(direction => direction.Count == 0);
            return moveDirections;
        }

        private bool PositionsHasEnemyPosition(List<Vector2Int> positions, List<ChessFigure> enemies) {
            foreach (var enemy in enemies) {
                if (positions.Contains(enemy.position)) {
                    return true;
                }
            }
            return false;
        }

        private void TraceMoveDirections() {
            List<Vector2Int> otherFiguresPositions = new List<Vector2Int>();
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
        }

        private void RemoveAllyPositions(ChessFigure figure) {
            var allyFigures = figures.FindAll(otherFigure => otherFigure.color == figure.color);
            foreach (var direciton in moveDirections) {
                foreach (var ally in allyFigures) {
                    direciton.Remove(ally.position);
                }
            }
        }


        private List<Vector2Int> CalculateDirection(DirectionParameters parameters) {
            List<Vector2Int> direction = new List<Vector2Int>();
            var positionX = parameters.start.x + parameters.offset.x;
            var positionY = parameters.start.y + parameters.offset.y;
            for (int i = 0; i < parameters.distance; i++) {
                if (positionX < 0 || positionX >= boardPositions.Length
                    || positionY < 0 || positionY >= boardPositions[0].Length) {
                    break;
                }
                direction.Add(new Vector2Int(positionX, positionY));
                positionX += parameters.offset.x;
                positionY += parameters.offset.y;
            }
            return direction;
        }
    }

}

