using UnityEngine;
using chess;
using System.Collections.Generic;

namespace chess {
    public class GameState : MonoBehaviour{
        public FigureColor currentPlayer;
        public List<ChessFigure> figures;
        public int movesWithoutFigureKilling;
    }
}