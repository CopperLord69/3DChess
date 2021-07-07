using ev;
using UnityEngine;

namespace Assets.Scripts.events.handlers {
    public class EscapeEvent : MonoBehaviour {
        public Event<EscEvent> handler;

        private void Awake() {
            handler = new Event<EscEvent>();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                handler.Invoke(new EscEvent { Button = new Key { pressed = true, released = false } });
            }
        }
    }

}


