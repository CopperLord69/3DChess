using System.Collections.Generic;
using UnityEngine;

namespace chess {
    public class FigureSet : MonoBehaviour {
        [HideInInspector]
        public List<GameObject> figures = new List<GameObject>();

        private void Awake() {
            var objects = GetComponentsInChildren<Transform>();
            foreach(var obj in objects) {
                if(obj.GetComponent<FigureType>() != null) {
                    figures.Add(obj.gameObject);
                }
            }
        }
    }
}