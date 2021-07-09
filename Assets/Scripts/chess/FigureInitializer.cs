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
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 2, fieldMask)) {
                figure.position = hit.collider.gameObject.name;
            }
        }

        public void SetFigureMaterials(Dictionary<FigureColor, Material> materials) {
            var material = materials[figure.color];
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) {
                renderer.material = material;
            }
        }
    }
}