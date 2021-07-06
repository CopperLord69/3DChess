using Assets.Scripts.events;
using Assets.Scripts.events.demuxes;
using ev;
using UnityEngine;

namespace Assets.Scripts.ui.controllers {
    public class MenuController : MonoBehaviour {

        [SerializeField]
        public GameObject menu;

        [HideInInspector]
        public EscapeDemux escDemux;

        private void Awake() {
            escDemux = new EscapeDemux();
            Token t = new Token();
            escDemux.demux.Register(t, ToggleMenu);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escDemux.demux.Push(new EscEvent { Button = new Key { pressed = true, released = false } });
            }
        }

        public void ToggleMenu(EscEvent escapeEvent) {

            menu.SetActive(!menu.activeSelf);
        }
    }

}

