using UnityEngine;

namespace chess {
    public struct PieceInfo {
        public GameObject piece;
        public Collider figureCollider;
        public FigureColor color;
        public Figure type;
    }
}