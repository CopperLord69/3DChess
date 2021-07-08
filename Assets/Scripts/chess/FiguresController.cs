using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class FiguresController {

        private List<ChessFigure> figures;

        private string[][] boardPositions;

        public FiguresController(List<ChessFigure> figures, string[][] boardPositions) {
            this.figures = figures;
            this.boardPositions = boardPositions;
        }

        public void CalculateFigureMoveDirections(ChessFigure figure) {
            int x = 0;
            int y = 0;
            while (x < boardPositions.GetLength(0)) {
                while (y < boardPositions.GetLength(1)) {
                    if (figure.position == boardPositions[x][y]) {
                        break;
                    }
                    y++;
                }
                x++;
            }
            switch (figure.type) {
                case Figure.Pawn: {
                        break;
                    }
                case Figure.Bishop: {
                        int distance = 8;
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, -1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, -1, distance));
                        break;
                    }
                case Figure.Rook: {
                        int distance = 8;
                        figure.moveDirections.Add(CalculateDirection(x, y, 0, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 0, -1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, 0, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, 0, distance));
                        break;
                    }
                case Figure.Queen: {
                        int distance = 8;
                        figure.moveDirections.Add(CalculateDirection(x, y, 0, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 0, -1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, 0, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, 0, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, -1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, -1, distance));
                        break;
                    }
                case Figure.Knight: {
                        figure.moveDirections.Add(CalculateDirection(x, y, 1,2 ,1));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1,-2 ,1));
                        figure.moveDirections.Add(CalculateDirection(x, y, 2,1 ,1));
                        figure.moveDirections.Add(CalculateDirection(x, y, 2,-1 ,1));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1,2 ,1));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1,-2 ,1));
                        figure.moveDirections.Add(CalculateDirection(x, y, -2,-1 ,1));
                        figure.moveDirections.Add(CalculateDirection(x, y, -2,1 ,1));
                        break;
                    }
                case Figure.King: {
                        int distance = 1;
                        figure.moveDirections.Add(CalculateDirection(x, y, 0, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 0, -1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, 0, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, 0, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, 1, -1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, 1, distance));
                        figure.moveDirections.Add(CalculateDirection(x, y, -1, -1, distance));
                        break;
                    }
                default: {
                        break;
                    }
            }
            RemoveAllyPositions(figure);
        }

        private void RemoveAllyPositions(ChessFigure figure) {
            foreach (var direction in figure.moveDirections) {
                List<string> busyPositions = new List<string>();
                foreach (var position in direction) {
                    foreach (var otherFigure in figures) {
                        if (otherFigure.position == position && otherFigure.type == figure.type) {
                            busyPositions.Add(position);
                        }
                    }
                }
                for (int i = 0; i < busyPositions.Count; i++) {
                    direction.Remove(busyPositions[i]);
                }
            }
        }


        private List<string> CalculateDirection(int startX, int startY, int offsetX, int offsetY, int distance) {
            List<string> direction = new List<string>();
            var positionX = startX + offsetX;
            var positionY = startY + offsetY;
            for (int i = 0; i < distance; i++) {
                if (positionX >= boardPositions.GetLength(0)
                    || positionY > boardPositions.GetLength(1)) {
                    break;
                }
                direction.Add(boardPositions[positionX][positionY]);
                positionX += offsetX;
                positionY += offsetY;
            }
            return direction;
        }
    }

}

