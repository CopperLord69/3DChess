using ev;
using UnityEngine;

namespace events.handlers {
    public class EscapeEvent : MonoBehaviour {
        public Demux<EscEvent> handler;

        private void Awake() {
            handler = new Demux<EscEvent>();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                handler.Push(new EscEvent { Button = new Key { pressed = true, released = false } });
            }
        }
    }

}


