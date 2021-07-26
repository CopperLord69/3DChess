using UnityEngine;
using chessEngn;

namespace chess {
    public struct PieceInfo {
        public GameObject piece;
        public Collider figureCollider;
        public FigureColor color;
        public Figure type;
    }
}