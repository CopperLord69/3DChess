using UnityEngine;

namespace chess {
    public class ChessFigure : MonoBehaviour{
        public Figure type;
        public FigureColor color;
        public Position position;
        public int movesCount;
        public bool madeTurnJustNow;
    }
}

