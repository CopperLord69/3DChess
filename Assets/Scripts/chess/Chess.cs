//public class Chess {






//    //public bool NoMovesAvailableForFiguresOfColor(FigureColor color) {
//    //    var allies = GetFiguresWithSameColor(color);
//    //    foreach (var ally in allies) {
//    //        var allyMoveDeirections = CalculateMovePositions(ally.position);
//    //        if (allyMoveDeirections.Count != 0) {
//    //            return false;
//    //        }
//    //    }
//    //    return true;
//    //}



//    //public bool CheckForCheck() {
//    //    var kings = GetKings();
//    //    foreach (var king in kings) {
//    //        if (IsKingInDanger(king)) {
//    //            return true;
//    //        }
//    //    }
//    //    return false;
//    //}

//    //public bool CheckForMate() {
//    //    var kings = GetKings();
//    //    foreach (var king in kings) {
//    //        if (IsKingInDanger(king)) {
//    //            if (NoMovesAvailableForFiguresOfColor(king.color)) {
//    //                return true;
//    //            }
//    //        }
//    //    }
//    //    return false;
//    //}

//    //public bool CheckForStalemate() {
//    //    var kings = GetKings();
//    //    foreach (var king in kings) {
//    //        if (NoMovesAvailableForFiguresOfColor(king.color)) {
//    //            return true;
//    //        }
//    //    }
//    //    return false;
//    //}

//    //public List<ChessFigure> GetFiguresWithOppositeColor(FigureColor color) {
//    //    List<ChessFigure> enemies = new List<ChessFigure>();
//    //    foreach (var figure in figures) {
//    //        if (figure.color != color) {
//    //            enemies.Add(figure);
//    //        }
//    //    }
//    //    return enemies;
//    //}

//    //public List<ChessFigure> GetFiguresWithSameColor(FigureColor color) {
//    //    List<ChessFigure> allies = new List<ChessFigure>();
//    //    foreach (var figure in figures) {
//    //        if (figure.color == color) {
//    //            allies.Add(figure);
//    //        }
//    //    }
//    //    return allies;
//    //}

//    //public List<ChessFigure> GetKings() {
//    //    var kings = new List<ChessFigure>();
//    //    foreach (var figure in figures) {
//    //        if (figure.type == Figure.King) {
//    //            kings.Add(figure);
//    //        }
//    //    }
//    //    return kings;
//    //}











//    //public Option<Position> GetKingInDangerPosition() {
//    //    var kings = GetKings();
//    //    foreach (var king in kings) {
//    //        if (IsKingInDanger(king)) {
//    //            return Option<Position>.Some(king.position);
//    //        }
//    //    }
//    //    return Option<Position>.None();
//    //}

//    //public List<Position> GetDangerPositions(List<Position> figureMovePositions) {
//    //    List<Position> dangerPositions = new List<Position>();
//    //    foreach (var figure in figures) {
//    //        if (figureMovePositions.Contains(figure.position)) {
//    //            dangerPositions.Add(figure.position);
//    //        }
//    //    }
//    //    return dangerPositions;
//    //}
//}
