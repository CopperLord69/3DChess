using ev;
using UnityEngine;

namespace events.handlers {
    public class MouseMoveEvent : MonoBehaviour {
        public Demux<MouseEvent> handler;

        private void Awake() {
            handler = new Demux<MouseEvent>();
        }

        private void Update() {
            var deltaMouseX = Input.GetAxis("Mouse X");
            if (deltaMouseX != 0) {
                handler.Push(new MouseEvent { axisX = deltaMouseX, axisY = 0 });
            }
        }
    }

}
