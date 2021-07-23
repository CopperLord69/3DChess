using UnityEngine;
using chess;
using System.Collections.Generic;

namespace chess {
    public class FigureInitializer : MonoBehaviour {

        private FigureType figure;
        
        [SerializeField]
        private List<FigureColor> colors;
        [SerializeField]
        private List<Material> materials;

        private Dictionary<FigureColor, Material> figureMaterials =
            new Dictionary<FigureColor, Material>();

        public bool alreadyHasPosition = false;

        private void Start() {
            for (int i = 0; i < colors.Count; i++) {
                figureMaterials.Add(colors[i], materials[i]);
            }
            figure = GetComponent<FigureType>();
            var material = materials[(int)figure.color];
            foreach (var renderer in GetComponentsInChildren<MeshRenderer>()) {
                renderer.material = material;
            }
        }

    }
}