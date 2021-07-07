using UnityEngine;

namespace Assets.Scripts.chess {
    public class ChessFigure : MonoBehaviour {
        public Figure type;
        public FigureColor color;
        public Vector3 position => transform.position;
    }
}

