using Assets.Scripts.events;
using Assets.Scripts.events.handlers;
using ev;
using UnityEngine;

namespace Assets.Scripts.ui.controllers {
    public class MenuController : MonoBehaviour {

        [SerializeField]
        public GameObject menu;
        [SerializeField]
        private EscapeEvent escEvent;

        private void Start() {
            Token t = new Token();
            escEvent.handler.AddListener(t, ToggleMenu);
        }
        public void ToggleMenu(EscEvent escapeEvent) {

            menu.SetActive(!menu.activeSelf);
            if (menu.activeSelf) {
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;

            }
        }

    }

}

