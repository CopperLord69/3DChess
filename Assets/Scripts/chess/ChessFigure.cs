using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class ChessFigure : MonoBehaviour {
        public Figure type;
        public FigureColor color;
        public string position;
        public List<List<string>> moveDirections = new List<List<string>>();
        public List<string> dangerPositions = new List<string>();
    }
}

