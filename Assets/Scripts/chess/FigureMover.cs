using UnityEngine;

namespace chess {
    public class FigureMover : MonoBehaviour {

        public Vector3 endPosition;
        private Transform trans;


        private void Start() {
            trans = transform;
        }


        void FixedUpdate() {
            if (Vector3.Distance(trans.localPosition, endPosition) > 0.1f) { 
                trans.localPosition = Vector3.Lerp(trans.localPosition, endPosition, 0.15f);
            } else {
                trans.position = endPosition;
                Destroy(this);
            }
        }
    }
}