using System.Collections;
using UnityEngine;

namespace chess {
    public class FigureMover : MonoBehaviour {

        public Vector3 endPosition;
        private Transform trans;


        private void Start() {
            trans = transform;
        }


        void FixedUpdate() {
            if (trans.localPosition != endPosition) {
                trans.localPosition = Vector3.Lerp(trans.localPosition, endPosition, 0.15f);
            } else {
                Destroy(this);
            }
        }
    }
}