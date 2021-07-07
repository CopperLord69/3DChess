using Assets.Scripts.chess;
using System.Collections.Generic;

namespace Assets.Scripts {
    public static class GameState {
        public static FigureColor currentPlayer;
        public static List<ChessFigure> blackFigures;
        public static List<ChessFigure> whiteFigures;
        public static int movesWithoutKills;
    }

}

