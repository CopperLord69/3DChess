using UnityEngine;
using chess;
using System.Collections.Generic;

namespace chess {
    public class FigureInitializer : MonoBehaviour {

        private ChessFigure figure;

        [SerializeField]
        private LayerMask fieldMask;

        [SerializeField]
        private List<FigureColor> colors;
        [SerializeField]
        private List<Material> materials;

        private Dictionary<FigureColor, Material> figureMaterials = 
            new Dictionary<FigureColor, Material>();


        private void Start() {
            for(int i = 0; i < colors.Count; i++) {
                figureMaterials.Add(colors[i], materials[i]);
            }
            figure = GetComponent<ChessFigure>();
            if(Physics.Raycast(transform.position, Vector3.down, 2, fieldMask)) {
                Vector3 position = transform.localPosition; 
                figure.position = new Vector2Int((int)position.x, (int)position.z);
            }
            SetFigureMaterials();
        }

        public void SetFigureMaterials() {
            var material = materials[(int)figure.color];
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) {
                renderer.material = material;
            }
        }
    }
}