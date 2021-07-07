using ev;
using UnityEngine;

namespace Assets.Scripts.events.handlers {
    public class MouseMoveEvent : MonoBehaviour{
        public Event<MouseEvent> handler;

        private void Awake() {
            handler = new Event<MouseEvent>();
        }

        private void Update() {
            var deltaMouseX = Input.GetAxis("Mouse X");
            if (deltaMouseX != 0) {
                handler.Invoke(new MouseEvent { axisX = deltaMouseX, axisY = 0 });
            }
        }
    }

}
