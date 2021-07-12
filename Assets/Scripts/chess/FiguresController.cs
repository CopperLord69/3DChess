using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class FiguresController {

        private List<ChessFigure> figures;

        private Vector2Int[][] boardPositions;

        private List<List<Vector2Int>> moveDirections = new List<List<Vector2Int>>();

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

            int x = figure.position.x;
            int y = figure.position.y;
            switch (figure.type) {
                case Figure.Pawn: {
                        var directionParameters = new DirectionParameters();
                        directionParameters.distance = 1;
                        directionParameters.start.x = x;
                        directionParameters.start.y = y;
                        if (figure.color == FigureColor.Black) {
                            if (figure.position.y == 6) {
                                directionParameters.offset.y = -1;
                                directionParameters.distance = 1;
                                moveDirections.Add(CalculateDirection(directionParameters));
                            }
                        } else {
                            if (figure.position.y == 1) {
                                directionParameters.offset.y = 1;
                                directionParameters.distance = 1;
                                moveDirections.Add(CalculateDirection(directionParameters));
                            }
                        }
                        var enemyFigures = figures.FindAll(other => other.color != figure.color);
                        bool hasNoEnemyNear = true;
                        for (int i = 0; hasNoEnemyNear && i < moveDirections.Count; i++) {
                            foreach (var enemy in enemyFigures) {
                                if (moveDirections[i].Contains(enemy.position)) {
                                    directionParameters.offset.x = -1;
                                    moveDirections.Add(CalculateDirection(directionParameters));
                                    directionParameters.offset.x = 1;
                                    moveDirections.Add(CalculateDirection(directionParameters));
                                    hasNoEnemyNear = false;
                                }
                            }
                        }
                        if (hasNoEnemyNear) {
                            directionParameters.distance = 2;
                            moveDirections.Add(CalculateDirection(directionParameters));
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
            TraceMoveDirections();
            RemoveAllyPositions(figure);
            return moveDirections;
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

