using UnityEngine;

namespace ui.controllers {
    public class MenuController : MonoBehaviour {

        [SerializeField]
        private GameObject menu;

        [SerializeField]
        private GameObject menuButton;

        [SerializeField]
        private FigurePicker figurePicker;

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                ToggleMenu();
            }
        }

        public void ToggleMenu() {
            figurePicker.enabled = !figurePicker.enabled;
            menu.SetActive(!menu.activeSelf);
            menuButton.SetActive(!menuButton.activeSelf);
            if (menu.activeSelf) {
                Time.timeScale = 0;
            } else {
                Time.timeScale = 1;
            }
        }

    }

}

