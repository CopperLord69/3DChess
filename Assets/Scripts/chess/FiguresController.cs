using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class FiguresController {

        private struct DirectionParameters {
            public int startX;
            public int startY;
            public int offsetX;
            public int offsetY;
            public int distance;

            public DirectionParameters(int startX,
                                       int startY,
                                       int offsetX,
                                       int offsetY,
                                       int distance) {
                this.startX = startX;
                this.startY = startY;
                this.offsetX = offsetX;
                this.offsetY = offsetY;
                this.distance = distance;

            }
        }

        private List<ChessFigure> figures;

        private string[][] boardPositions;

        public FiguresController(List<ChessFigure> figures, string[][] boardPositions) {
            this.figures = figures;
            this.boardPositions = boardPositions;
        }

        public void CalculateFigureMoveDirections(ChessFigure figure) {
            int x = 0;
            int y = 0;
            figure.moveDirections.Clear();
            while (x < boardPositions.Length - 1) {
                x++;
                y = 0;
                while (y < boardPositions[x].Length) {
                    if (figure.position == boardPositions[x][y]) {
                        break;
                    }
                    y++;
                }
            }
            switch (figure.type) {
                case Figure.Pawn: {
                        break;
                    }
                case Figure.Bishop: {
                        int distance = 8;
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            -1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            -1,
                            distance)));
                        break;
                    }
                case Figure.Rook: {
                        int distance = 8;
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            0,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            0,
                            -1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            0,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            0,
                            distance)));
                        break;
                    }
                case Figure.Queen: {
                        int distance = 8;
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            0,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            0,
                            -1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            0,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            0,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            -1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            -1,
                            distance)));
                        break;
                    }
                case Figure.Knight: {
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            2,
                            1)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            -2,
                            1)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            2,
                            1,
                            1)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            2,
                            -1,
                            1)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            2,
                            1)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            -2,
                            1)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -2,
                            -1,
                            1)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            
                            x,
                            y,
                            -2,
                            1,
                            1)));
                        break;
                    }
                case Figure.King: {
                        int distance = 1;
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            0,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            0,
                            -1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            0,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            0,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            1,
                            -1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            1,
                            distance)));
                        figure.moveDirections.Add(CalculateDirection(new DirectionParameters(
                            x,
                            y,
                            -1,
                            -1,
                            distance)));
                        break;
                    }
                default: {
                        break;
                    }
            }
            TraceMoveDirections(figure);
            RemoveAllyPositions(figure);
        }

        private void TraceMoveDirections(ChessFigure figure) {
            List<string> otherFiguresPositions = new List<string>();
            foreach (var otherFigure in figures) {
                otherFiguresPositions.Add(otherFigure.position);
            }
            foreach (var otherFigurePosition in otherFiguresPositions) {
                foreach (var direction in figure.moveDirections) {
                    if (direction.Contains(otherFigurePosition)) {
                        int index = direction.IndexOf(otherFigurePosition);
                        direction.RemoveRange(index, direction.Count - index);
                    }
                }
            }
        }

        private void RemoveAllyPositions(ChessFigure figure) {
            foreach (var direction in figure.moveDirections) {
                List<string> busyPositions = new List<string>();
                foreach (var position in direction) {
                    foreach (var otherFigure in figures) {
                        if (otherFigure.position == position && otherFigure.color == figure.color) {
                            busyPositions.Add(position);
                        }
                    }
                }
                foreach (var pos in busyPositions) {
                    direction.Remove(pos);
                }
            }
        }


        private List<string> CalculateDirection(DirectionParameters parameters) {
            List<string> direction = new List<string>();
            var positionX = parameters.startX + parameters.offsetX;
            var positionY = parameters.startY + parameters.offsetY;
            for (int i = 0; i < parameters.distance; i++) {
                if (positionX < 0 || positionX >= boardPositions.Length
                    || positionY < 0 || positionY >= boardPositions[0].Length) {
                    break;
                }
                direction.Add(boardPositions[positionX][positionY]);
                positionX += parameters.offsetX;
                positionY += parameters.offsetY;
            }
            return direction;
        }
    }

}

