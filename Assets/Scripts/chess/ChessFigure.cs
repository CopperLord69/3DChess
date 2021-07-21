using UnityEngine;

namespace chess {
    public class ChessFigure : MonoBehaviour{
        public Figure type;
        public FigureColor color;
        public Vector2Int position;
        public int movesCount;
        public bool madeTurnJustNow;
    }
}

