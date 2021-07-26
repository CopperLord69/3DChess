using option;
using System.Collections.Generic;
using UnityEngine;

namespace chessEngn {

    public class ChessFigure {
        public Figure type;
        public FigureColor color;
        public Position position;
        public int movesCount;
        public bool madeTurnJustNow;
    }

    public class Chess {

        private List<ChessFigure> figures;
        public Chess() {
            figures = new List<ChessFigure> {
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 0, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 1, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 2, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 3, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 4, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 5, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 6, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.White,
                position = new Position() {x = 7, y = 1},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 7, y = 6},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 6, y = 6},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 5, y = 6},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 4, y = 6},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 3, y = 6},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 2, y = 6},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 1, y = 6},
            },
            new ChessFigure {
                type = Figure.Pawn,
                color = FigureColor.Black,
                position = new Position() {x = 0, y = 6},
            },
            new ChessFigure {
                type = Figure.Rook,
                color = FigureColor.White,
                position = new Position { x = 0, y = 0},
            },
            new ChessFigure {
                type = Figure.Rook,
                color = FigureColor.White,
                position = new Position { x = 7, y = 0},
            },
            new ChessFigure {
                type = Figure.Rook,
                color = FigureColor.Black,
                position = new Position { x = 0, y = 7},
            },
            new ChessFigure {
                type = Figure.Rook,
                color = FigureColor.Black,
                position = new Position { x = 7, y = 7},
            },
            new ChessFigure {
                type = Figure.Knight,
                color = FigureColor.White,
                position = new Position { x = 1, y = 0},
            },
            new ChessFigure {
                type = Figure.Knight,
                color = FigureColor.White,
                position = new Position { x = 6, y = 0},
            },
            new ChessFigure {
                type = Figure.Knight,
                color = FigureColor.Black,
                position = new Position { x = 6, y = 7},
            },
            new ChessFigure {
                type = Figure.Knight,
                color = FigureColor.Black,
                position = new Position { x = 1, y = 7},
            },
            new ChessFigure {
                type = Figure.Bishop,
                color = FigureColor.White,
                position = new Position {x = 2, y = 0 }
            },
            new ChessFigure {
                type = Figure.Bishop,
                color = FigureColor.White,
                position = new Position {x = 5, y = 0 }
            },
            new ChessFigure {
                type = Figure.Queen,
                color = FigureColor.White,
                position = new Position {x = 3, y = 0 }
            },
            new ChessFigure {
                type = Figure.King,
                color = FigureColor.White,
                position = new Position {x = 4, y = 0 }
            },
            new ChessFigure {
                type = Figure.Bishop,
                color = FigureColor.Black,
                position = new Position {x = 2, y = 7 }
            },
            new ChessFigure {
                type = Figure.Bishop,
                color = FigureColor.Black,
                position = new Position {x = 5, y = 7 }
            },
            new ChessFigure {
                type = Figure.Queen,
                color = FigureColor.Black,
                position = new Position {x = 3, y = 7 }
            },
            new ChessFigure {
                type = Figure.King,
                color = FigureColor.Black,
                position = new Position {x = 4, y = 7 }
            },
        };
        }

        public Chess(List<ChessFigure> figures) {
            this.figures = figures;
        }

        public List<ChessFigure> GetFigures() {
            return figures;
        }

        public void DeleteFigureWithPosition(Position pos) {
            foreach (var figure in figures) {
                if (Position.AreSame(pos, figure.position)) {
                    figures.Remove(figure);
                    break;
                }
            }
        }

        public void SetFigurePosition(Position figurePosition, Position newPosition) {
            var figureOption = GetFigureOnPosition(figurePosition);
            foreach (var figure in figures) {
                figure.madeTurnJustNow = false;
            }
            if (figureOption.IsSome()) {
                figureOption.Peel().movesCount += 1;
                figureOption.Peel().madeTurnJustNow = true;
                figureOption.Peel().position = newPosition;
            }
        }

        public void TransformFigure(Position figurePosition, Figure newType) {
            var figureOption = GetFigureOnPosition(figurePosition);
            if (figureOption.IsSome()) {
                figureOption.Peel().type = newType;
            }
        }

        public Dictionary<Position, Position> GetCastlingPositions(Position kingPosition) {
            var kingOp = GetFigureOnPosition(kingPosition);
            Dictionary<Position, Position> castlingPositionsToReturn =
                new Dictionary<Position, Position>();
            if (kingOp.IsSome()) {
                var king = kingOp.Peel();
                Dictionary<Position, Position> castlingPositions = CalculateCastlings(king);
                if (king.movesCount == 0) {
                    foreach (var pos in castlingPositions) {
                        var figure = GetFigureOnPosition(castlingPositions[pos.Key]);
                        if (figure.IsSome()) {
                            if (figure.Peel().movesCount != 0) {
                                continue;
                            }
                        } else { 
                            continue; 
                        }
                        var allies = GetFiguresWithSameColor(king.color);
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

        private Dictionary<Position, Position> CalculateCastlings(ChessFigure figure) {
            Dictionary<Position, Position> castlingPositions =
                new Dictionary<Position, Position>();
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
            castlingPositions.Add(leftKingPosition, leftRookPosition);
            castlingPositions.Add(rightKingPosition, rightRookPosition);
            foreach (var fig in figures) {
                if (Position.AreSame(fig.position, leftRookPosition)) {
                }
                if (Position.AreSame(fig.position, rightRookPosition)) {
                }
            }
            return castlingPositions;
        }

        public Dictionary<Position, Position> GetPawnEnPassants(Position pawnPosition) {
            Dictionary<Position, Position> approachPositions = new Dictionary<Position, Position>();
            var pawnOp = GetFigureOnPosition(pawnPosition);
            if (pawnOp.IsSome()) {
                var pawn = pawnOp.Peel();
                var enemies = GetFiguresWithOppositeColor(pawn.color);
                int enemyFirstMoveLine;
                int xOffset;
                if (pawn.color == FigureColor.White) {
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
                        if (Mathf.Abs(enemy.position.x - pawn.position.x) < 2) {
                            bool isPawnWithOneDouble = (enemy.type == Figure.Pawn);
                            isPawnWithOneDouble &= enemy.movesCount == 1;
                            isPawnWithOneDouble &= enemy.madeTurnJustNow;
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

        public bool NoMovesAvailableForFiguresOfColor(FigureColor color) {
            var allies = GetFiguresWithSameColor(color);
            foreach (var ally in allies) {
                var allyMoveDeirections = CalculateMovePositions(ally.position);
                if (allyMoveDeirections.Count != 0) {
                    return false;
                }
            }
            return true;
        }

        private bool IsKingInDanger(ChessFigure king) {
            List<ChessFigure> enemies = GetFiguresWithOppositeColor(king.color);
            foreach (var enemy in enemies) {
                var enemyMoveDirecitons = CalculateFigureMoveDirections(enemy);
                foreach (var enemyDirection in enemyMoveDirecitons) {
                    if (enemyDirection.Contains(king.position)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckForCheck() {
            var kings = GetKings();
            foreach (var king in kings) {
                if (IsKingInDanger(king)) {
                    return true;
                }
            }
            return false;
        }

        public bool CheckForMate() {
            var kings = GetKings();
            foreach (var king in kings) {
                if (IsKingInDanger(king)) {
                    if (NoMovesAvailableForFiguresOfColor(king.color)) {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckForStalemate() {
            var kings = GetKings();
            foreach (var king in kings) {
                if (NoMovesAvailableForFiguresOfColor(king.color)) {
                    return true;
                }
            }
            return false;
        }

        public List<ChessFigure> GetFiguresWithOppositeColor(FigureColor color) {
            List<ChessFigure> enemies = new List<ChessFigure>();
            foreach (var figure in figures) {
                if (figure.color != color) {
                    enemies.Add(figure);
                }
            }
            return enemies;
        }

        public List<ChessFigure> GetFiguresWithSameColor(FigureColor color) {
            List<ChessFigure> allies = new List<ChessFigure>();
            foreach (var figure in figures) {
                if (figure.color == color) {
                    allies.Add(figure);
                }
            }
            return allies;
        }

        public List<ChessFigure> GetKings() {
            var kings = new List<ChessFigure>();
            foreach (var figure in figures) {
                if (figure.type == Figure.King) {
                    kings.Add(figure);
                }
            }
            return kings;
        }

        public Option<ChessFigure> FindKingWithColor(FigureColor color) {
            foreach (var figure in figures) {
                if (figure.type == Figure.King && figure.color == color) {
                    return Option<ChessFigure>.Some(figure);
                }
            }
            return Option<ChessFigure>.None();
        }

        public List<Position> CalculateMovePositions(Position figurePosition) {
            var figureOp = GetFigureOnPosition(figurePosition);
            var figureMovePositions = new List<Position>();
            if (figureOp.IsSome()) {
                var figure = figureOp.Peel();
                var allyKing = FindKingWithColor(figure.color);
                var enemies = GetFiguresWithOppositeColor(figure.color);
                var impossiblePositions = new List<Position>();
                Position initialPosition = figure.position;
                var figureMoveDirections = CalculateFigureMoveDirections(figure);
                foreach (var direction in figureMoveDirections) {
                    foreach (var position in direction) {
                        if (allyKing.IsSome()) {
                            List<ChessFigure> tempEnemies = new List<ChessFigure>();
                            foreach (var enemy in enemies) {
                                if (Position.AreSame(position, enemy.position)) {
                                    figures.Remove(enemy);
                                    tempEnemies.Add(enemy);
                                }
                            }
                            figure.position = position;
                            if (IsKingInDanger(allyKing.Peel())) {
                                impossiblePositions.Add(position);
                            }
                            foreach (var enemy in tempEnemies) {
                                figures.Add(enemy);
                            }
                        }
                    }
                    foreach (var position in impossiblePositions) {
                        direction.Remove(position);
                    }
                }
                figure.position = initialPosition;
                foreach (var figureDirection in figureMoveDirections) {
                    figureMovePositions.AddRange(figureDirection);
                }
            }
            return figureMovePositions;
        }

        private List<List<Position>> CalculateFigureMoveDirections(ChessFigure figure) {
            var moveDirections = new List<List<Position>>();
            int x = figure.position.x;
            int y = figure.position.y;
            switch (figure.type) {
                case Figure.Pawn: {
                        var enemyFigures = GetFiguresWithOppositeColor(figure.color);
                        var directionParameters = new DirectionParameters {
                            distance = 1,
                            start = new Vector2Int(x, y)
                        };
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
                            if (!HasEnemyOnDirection(direction, enemyFigures)) {
                                direction.Clear();
                            }
                        }
                        directionParameters.offset.x = 0;
                        var positions = CalculateDirection(directionParameters);
                        if (!HasEnemyOnDirection(positions, enemyFigures)) {
                            moveDirections.Add(positions);
                            if (figure.position.y == firstMoveLine) {
                                directionParameters.distance = 2;
                                var farPosition = CalculateDirection(directionParameters);
                                if (!HasEnemyOnDirection(farPosition, enemyFigures)) {
                                    moveDirections.Add(farPosition);
                                }
                            }
                        }
                        foreach (var key in GetPawnEnPassants(figure.position).Keys) {
                            moveDirections.Add(new List<Position> { key });
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
                        var castlings = GetCastlingPositions(figure.position);
                        foreach (var pos in castlings) {
                            moveDirections.Add(new List<Position> { pos.Key });
                        }
                        break;
                    }
                default: {
                        break;
                    }
            }
            moveDirections = TraceMoveDirections(moveDirections);
            moveDirections = RemoveAllyPositions(figure, moveDirections);
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

        private List<Position> CalculateDirection(DirectionParameters parameters) {
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

        private bool HasEnemyOnDirection(
            List<Position> direction,
            List<ChessFigure> enemies
        ) {
            foreach (var enemy in enemies) {
                if (direction.Contains(enemy.position)) {
                    return true;
                }
            }
            return false;
        }

        private List<List<Position>> TraceMoveDirections(List<List<Position>> moveDirections) {
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

        private List<List<Position>> RemoveAllyPositions(
            ChessFigure figure,
            List<List<Position>> moveDirections
        ) {
            var allyFigures = GetFiguresWithSameColor(figure.color);
            foreach (var direciton in moveDirections) {
                foreach (var ally in allyFigures) {
                    direciton.Remove(ally.position);
                }
            }
            return moveDirections;
        }

        public Option<ChessFigure> GetFigureOnPosition(Position position) {
            foreach (var figure in figures) {
                if (Position.AreSame(figure.position, position)) {
                    return Option<ChessFigure>.Some(figure);
                }
            }
            return Option<ChessFigure>.None();
        }

        public Option<Position> GetKingInDangerPosition() {
            var kings = GetKings();
            foreach (var king in kings) {
                if (IsKingInDanger(king)) {
                    return Option<Position>.Some(king.position);
                }
            }
            return Option<Position>.None();
        }

        public List<Position> GetDangerPositions(List<Position> figureMovePositions) {
            List<Position> dangerPositions = new List<Position>();
            foreach (var figure in figures) {
                if (figureMovePositions.Contains(figure.position)) {
                    dangerPositions.Add(figure.position);
                }
            }
            return dangerPositions;
        }
    }
}