using Assets.Scripts.events;
using Assets.Scripts.events.handlers;
using ev;
using UnityEngine;

namespace Assets.Scripts.ui.controllers {
    public class MenuController : MonoBehaviour {

        [SerializeField]
        public GameObject menu;

        private EscapeEvent escEvent;

        private void Awake() {
            escEvent = new EscapeEvent();
            Token t = new Token();
            escEvent.handler.AddListener(t, ToggleMenu);
        }
        public void ToggleMenu(EscEvent escapeEvent) {

            menu.SetActive(!menu.activeSelf);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escEvent.handler.Invoke(new EscEvent { Button = new Key { pressed = true, released = false } });
            }
        }

    }

}

