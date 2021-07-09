using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class FigureSet : MonoBehaviour {
        [HideInInspector]
        public List<ChessFigure> figures = new List<ChessFigure>();

        private void Start() {
            var objects = GetComponentsInChildren<Transform>();
            foreach(var obj in objects) {
                if(obj.TryGetComponent(out ChessFigure figure)) {
                    figures.Add(figure);
                }
            }
        }
    }
}